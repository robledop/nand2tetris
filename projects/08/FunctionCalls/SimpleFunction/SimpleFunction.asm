(SimpleFunction.test)        // line = function SimpleFunction.test 2
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
@LCL      // line = push local 0
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1
@LCL      // line = push local 1
A=M
A=A+1
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP      // line = add
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
@SP      // line = not
A=M-1
M=!M
@ARG      // line = push argument 0
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP      // line = add
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
@ARG      // line = push argument 1
A=M
A=A+1
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP      // line = sub
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
@LCL        // line = return
D=M
@SimpleFunction$FRAME.0
M=D // FRAME = LCL
@5
D=A
@SimpleFunction$FRAME.0
D=M-D
A=D
D=M
@SimpleFunction$RET.0
M=D      // RET = *(FRAME - 5)
@SP
M=M-1
A=M
D=M
@ARG
A=M
M=D      // *ARG = pop()
@ARG
D=M
@SP
M=D+1    // SP = ARG + 1
@1
D=A
@SimpleFunction$FRAME.0
D=M-D
A=D
D=M
@THAT
M=D      // THAT = *(FRAME-1)
@2
D=A
@SimpleFunction$FRAME.0
D=M-D
A=D
D=M
@THIS
M=D      // THIS = *(FRAME-2)
@3
D=A
@SimpleFunction$FRAME.0
D=M-D
A=D
D=M
@ARG
M=D      // ARG = *(FRAME-3)
@4
D=A
@SimpleFunction$FRAME.0
D=M-D
A=D
D=M
@LCL
M=D      // LCL = *(FRAME-4)
@SimpleFunction$RET.0
A=M
0;JMP        // goto RET