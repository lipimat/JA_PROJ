.data
pixels dq 0
pixelsWidth dd 0
pixelsHeight dd 0
pixelsCount dd 0
matrix dq 0

.code
asmProc proc
mov rax, rcx
mov rbx, [rcx]				; ardes pierwszej struktury piksla w tablicy
mov pixels, rbx
mov ebx, [rcx+8]			; szerokosc obrazu w piklsach
mov pixelsWidth, ebx
mov ebx, [rcx+12]			; wysokosc obrazu w pikslach
mov pixelsHeight, ebx
mov rbx, [rcx+16]			; wskaznik na macierz transformacji
mov matrix, rbx

mov eax, pixelsWidth		; policz ile jest piksli
mul pixelsHeight
mov pixelsCount, eax

; zmien piksle na niebieskie
mov rax, pixels				; wskaznik na strukturê (pierwszy piksel)
mov ecx, 0					; wyzeruj licznik
Iteration:	
	mov byte ptr [rax], 255		; niebieski = 255
	mov byte ptr [rax + 1], 0	; zielony = 0
	mov byte ptr [rax + 2], 0	; czerwony = 0
								; nie dotykamy alpha
	add rax, 4				; wskaz na nastepny piksel (struktura ma w sobie 4 wskazniki na byte dlatego przesuwamy sie o 4 adresy dalej)
	inc ecx					; zwieksz licznik
	cmp ecx, pixelsCount	; porownaj licznik z dlugoscia tablicy piksli
	jl Iteration			; jak < skocz do loop i licz dalej
ret
asmProc endp
end