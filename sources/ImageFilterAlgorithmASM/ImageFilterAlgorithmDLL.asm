.data
pixels dq 0
pixelsLen dd 0
matrix dq 0

.code
asmProc proc
mov rax, rcx
mov rbx, [rcx]		;ardes pierwszego wskaznika w tablicy (wskaznik na piksle)
mov pixels, rbx
mov ebx, [rcx+8]	;dlugosc tablicy piksli
mov pixelsLen, ebx
mov rbx, [rcx+16]	;wskaznik na macierz transformacji
mov matrix, rbx

; zmien piksle na niebieskie
mov rax, pixels		; wskaznik na pierwsza skladowa (niebieska) pierwszego elementu
mov ecx, 0			; wyzeruj licznik
mov bl, 255			; narazie wszedzie wpiszemy 255
Iteration:
	mov [rax], bl			; wpisz 255 do niebieskiego
	add al, 4				; idz 4 adresy dalej
	add cl, 4 				; inkrementacja do kolejnego piksla (skladowa niebieska) 
	cmp ecx, pixelsLen		; porownaj licznik z dlugoscia tablicy piksli
	jl Iteration			; jak < skocz do loop i licz dalej
ret
asmProc endp
end