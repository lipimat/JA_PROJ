.data
ptr1 dq 0
len1 dd 0

.code
asmProc proc
mov rax, rcx
mov rbx, [rcx] ;ardes pierwszego wskaznika w tablicy
mov ptr1, rbx
mov ebx, [rcx+8] ;dlugosc tablicy piksli
mov len1, ebx
ret
asmProc endp
end