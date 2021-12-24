.data
originalBytes		dq 0
resultBytes			dq 0
countOfBytesInRow	dq 0
imageHeight			dd 0
matrix				dq 0
checkSum			dq 0

loopCounter			dq 0
loopEnd				dq 0
alphaCounter		dd 1
lineCounter			dq 5

byteHandler			db 16 DUP(0)
matrixHandler		sword 8 DUP(0)
result				sdword 0
finalRes			sword 0
MMX_XOR				dword 65535
temp				dword 0
.code
asmProc proc
; -------------fetch data-------------

mov rax, rcx
mov rbx, [rcx]					; wskaznik na tablice oryginalnych bajtow
mov originalBytes, rbx
mov rbx, [rcx+8]				; wskaznik na tablice bajtow ktora bedziemy edytowac
mov resultBytes, rbx
mov ebx, [rcx+16]				; ilosc bajtow w jednym wierszu obrazka
mov dword ptr countOfBytesInRow, ebx
mov ebx, [rcx+20]				; wysokosc obrazka
mov imageHeight, ebx
mov rbx, [rcx+24]				; wskaznik na kernel (macierz przeksztalcenia)
mov matrix, rbx
mov ebx, [rcx+32]				; suma wartosci wszystkich elementow macierzy
mov dword ptr checkSum, ebx

; -------------fetch data-------------

; -------------prepare for calculations-------------
cmp checkSum, 0
jne Cont
mov checkSum, 1							; jezeli suma wartosci macierzy = 0, to zamien na 1 zeby uniknac dzielenia przez 0
Cont:
xor rcx, rcx
mov rcx, countOfBytesInRow
add rcx, 4								
mov loopCounter, rcx					; inicjalizacja licznika petli

xor rax, rax
mov eax, imageHeight
dec eax
mul countOfBytesInRow					
mov loopEnd, rax						; inicjalizacja wartosci do zakonczena petli

mov rbx, [originalBytes]				; trzymamy wskaznik do oryginalnych bajtow w rbx
mov rcx, [resultBytes]					; trzymamy wskaznik do edytowanych bajtow w rcx
mov rdx, [matrix]						; trzymamy wskaznik do macierzy (kernela) w rdx

mov ax, sword ptr [rdx]
mov matrixHandler, ax					; matrix[0][0]
mov ax, sword ptr [rdx + 12]
mov matrixHandler+2, ax					; matrix[0][1]
mov ax, sword ptr [rdx + 24]
mov matrixHandler+4, ax					; matrix[0][2]
mov ax, sword ptr [rdx + 4]
mov matrixHandler+6, ax					; matrix[1][0]
mov ax, sword ptr [rdx + 16]
mov matrixHandler+8, ax					; matrix[1][1]
mov ax, sword ptr [rdx + 28]
mov matrixHandler+10, ax				; matrix[1][2]
mov ax, sword ptr [rdx + 8]
mov matrixHandler+12, ax				; matrix[2][0]
mov ax, sword ptr [rdx + 20]
mov matrixHandler+14, ax				; matrix[2][1]				
movups xmm1, xmmword ptr matrixHandler	; ustaw w odpowiedniej kolejosci wartosci kernela i wpisz do xmm1
; -------------prepare for calculations-------------

; -------------calculations-------------
MainLoop: 
mov rax, loopCounter
cmp rax, loopEnd
jg Exit									; sprawdz czy konczymy petle, jesli nie to kontynuuj
mov rax, countOfBytesInRow
sub rax, 4
cmp rax, lineCounter
jne Continue1							; sprawdz czy trafilismy na ostatni piksel w rzedzie, jesli tak pomin 2 piksle
mov lineCounter, 5
add loopCounter, 8

Continue1:
cmp alphaCounter, 4
jne Continue2							; sprawdz czy teraz przetwarzamy bajt alpha, jesli tak to pomin bajt 
mov alphaCounter, 1
inc lineCounter
inc loopCounter

Continue2:
inc lineCounter
inc alphaCounter

mov r10, loopCounter
add r10, rbx
sub r10, countOfBytesInRow				; w r10 adres piksla nad nami

mov al, byte ptr [r10 - 4]				
mov byteHandler, al						; wartosc z lewego gornego rogu
mov al, byte ptr [r10]
mov byteHandler+2, al					; wartosc z gory
mov al, byte ptr [r10 + 4]
mov byteHandler+4, al					; wartosc z prawego gornego rogu

add r10, countOfBytesInRow				; w r10 adres teraz przetwarzanego piksla

mov al, byte ptr [r10 - 4]				
mov byteHandler+6, al					; wartosc z lewej strony
mov al, byte ptr [r10]
mov byteHandler+8, al					; wartosc ktora aktualnie filtrujemy
mov al, byte ptr [r10 + 4]
mov byteHandler+10, al					; wartosc z prawej strony

add r10, countOfBytesInRow				; w r10 adres piksla pod nami

mov al, byte ptr [r10 - 4]				
mov byteHandler+12, al					; wartosc z lewego dolnego rogu
mov al, byte ptr [r10]
mov byteHandler+14, al					; wartosc z dolu

movups xmm2, xmmword ptr byteHandler
movups xmm3, xmmword ptr byteHandler
pxor xmm4, xmm4
pxor xmm5, xmm5

pmulhw xmm2, xmm1						; zapisz starsze slowo wyniku mnozenia wartosci przez macierz
pmullw xmm3, xmm1						; zapisz mlodsze slowo wyniku mnozenia wartosci przez macierz
paddsw xmm3, xmm2						; sumuj (zostalo 8 wartosci do zsumowania)
movhlps xmm4, xmm3						; przenies gorne 64 bity z xmm3 do dolnych 64 bitow z xmm4
paddsw xmm4, xmm3						; sumuj (zostaly 4 wartosci do zsumowania)
movlhps xmm5, xmm4						; przenies dolne 64 bity z xmm4 do gornych 64 bitow z xmm5
psrldq xmm5, 12							; przesun xmm5 w prawo o 3 * podwojne slowo
paddsw xmm5, xmm4						; sumuj (zostaly 2 wartosci do zsumowania)
movss xmm6, xmm5						; przenies dolne 32 bity z xmm5 do dolnych 32 bitow z xmm6
psrldq xmm6, 2							; przesun xmm6 w prawo o jedno slowo
paddsw xmm6,xmm5						; sumuj (najmlodze slowo w xmm6 to wynik)

pxor xmm7, xmm7
movss xmm7, MMX_XOR
pand xmm6, xmm7							; wyzeruj reszte xmm6, nie jest potrzebna

xor rax, rax
mov al, byte ptr [r10 + 4]
mul sword ptr [rdx + 32]				; matrix[2][2], przemnazamy ostatnia wartosc macierzy z wartoscia z prawego dolnego rogu
mov temp, eax
movss xmm7, temp
paddsw xmm6, xmm7						; sumuj (mamy cala wartosc, teraz tylko podzielic przez checkSume)
movd result, xmm6						; zapisz wynik w results

mov eax, result
mov edx, 0								; potrzebne do diva
div checkSum							; podziel wyliczona wyzej wartosc przez checkSume
mov finalRes, ax						; finalna wartosc nowego bajtu (wynik bedzie w al)
mov rdx, [matrix]						; trzymamy wskaznik do macierzy (kernela) w rdx

pxor xmm2, xmm2
pxor xmm3, xmm3
pxor xmm4, xmm4
pxor xmm5, xmm5
pxor xmm6, xmm6
pxor xmm7, xmm7							; wyzerowanie rejestrow dla pewnosci

cmp finalRes, 0
jg Continue3							; jezeli finalna wartosc < 0 to wpisz w 0 w al 
mov al, 0

Continue3: 
mov r11, rcx
add r11, loopCounter					; adres piksla ktoremu wpiszemy nowa wartosc
mov byte ptr [r11], al					; wpisz wartosc nowego bajtu

inc loopCounter
jmp MainLoop							; skok do poczatku

Exit:
mov alphaCounter, 1						
mov lineCounter, 5						; przypisanie wartosci spowrotem do zmiennych lokalnych
ret
; -------------calculations-------------
asmProc endp
end