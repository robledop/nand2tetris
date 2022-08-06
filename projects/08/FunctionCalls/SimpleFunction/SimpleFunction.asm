// function SimpleFunction.test 2
(SimpleFunction.test)
@SP
A=M
M=0
@SP
M=M+1
@SP
A=M
M=0
@SP
M=M+1

// push local 0
@LCL
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1

// push local 1
@LCL
A=M
A=A+1
D=M
@SP
A=M
M=D
@SP
M=M+1

// add
@SP
M=M-1
@SP
A=M
D=M
@SP
M=M-1
@SP
A=M
M=M+D
@SP
M=M+1

// not
@SP
A=M-1
M=!M

// push argument 0
@ARG
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1

// add
@SP
M=M-1
@SP
A=M
D=M
@SP
M=M-1
@SP
A=M
M=M+D
@SP
M=M+1

// push argument 1
@ARG
A=M
A=A+1
D=M
@SP
A=M
M=D
@SP
M=M+1

// sub
@SP
M=M-1
@SP
A=M
D=M
@SP
M=M-1
@SP
A=M
M=M-D
@SP
M=M+1

// return
// FRAME = LCL
@LCL
D=M
@FRAME
M=D
// RET = *(FRAME - 5)
@5
D=A
@FRAME
D=M-D
A=D
D=M
@RET
M=D
// *ARG = pop()
@SP
M=M-1
A=M
D=M
@ARG
A=M
M=D
// SP = ARG + 1
@ARG
D=M
@SP
M=D+1
// THAT = *(FRAME-1)
@1
D=A
@FRAME
D=M-D
A=D
D=M
@THAT
M=D
// THIS = *(FRAME-2)
@2
D=A
@FRAME
D=M-D
A=D
D=M
@THIS
M=D
// ARG = *(FRAME-3)
@3
D=A
@FRAME
D=M-D
A=D
D=M
@ARG
M=D
// LCL = *(FRAME-4)
@4
D=A
@FRAME
D=M-D
A=D
D=M
@LCL
M=D
// goto RET
@RET
A=M
0;JMP
