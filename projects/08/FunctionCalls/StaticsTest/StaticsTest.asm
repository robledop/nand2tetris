@256
D=A
@SP
M=D         // bootstrap
@RET_ADDRESS.Sys.init     // line = call Sys.init 0
D=A
@SP
A=M
M=D
@SP
M=M+1        // push return-address
@LCL
D=M
@SP
A=M
M=D
@SP
M=M+1        // push LCL
@ARG
D=M
@SP
A=M
M=D
@SP
M=M+1        // push ARG
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THIS
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THAT
@SP
D=M
@0
D=D-A
@5
D=D-A
@ARG
M=D          // ARG=SP-n-5
@SP
D=M
@LCL
M=D          // LCL=SP
@Sys.init
0;JMP
(RET_ADDRESS.Sys.init)
(Class1.set)        // line = function Class1.set 0
@ARG      // line = push argument 0
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop static 0
M=M-1
@SP
A=M
D=M
@Class1.0
M=D
@ARG      // line = push argument 1
A=M
A=A+1
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop static 1
M=M-1
@SP
A=M
D=M
@Class1.1
M=D
@0     // line = push constant 0
D=A
@SP
A=M
M=D
@SP
M=M+1
@LCL        // line = return
D=M
@Class1$FRAME.0
M=D // FRAME = LCL
@5
D=A
@Class1$FRAME.0
D=M-D
A=D
D=M
@Class1$RET.0
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
@Class1$FRAME.0
D=M-D
A=D
D=M
@THAT
M=D      // THAT = *(FRAME-1)
@2
D=A
@Class1$FRAME.0
D=M-D
A=D
D=M
@THIS
M=D      // THIS = *(FRAME-2)
@3
D=A
@Class1$FRAME.0
D=M-D
A=D
D=M
@ARG
M=D      // ARG = *(FRAME-3)
@4
D=A
@Class1$FRAME.0
D=M-D
A=D
D=M
@LCL
M=D      // LCL = *(FRAME-4)
@Class1$RET.0
A=M
0;JMP        // goto RET
(Class1.get)        // line = function Class1.get 0
@Class1.0     // line = push static 0
D=M
@SP
A=M
M=D
@SP
M=M+1
@Class1.1     // line = push static 1
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
@Class1$FRAME.1
M=D // FRAME = LCL
@5
D=A
@Class1$FRAME.1
D=M-D
A=D
D=M
@Class1$RET.1
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
@Class1$FRAME.1
D=M-D
A=D
D=M
@THAT
M=D      // THAT = *(FRAME-1)
@2
D=A
@Class1$FRAME.1
D=M-D
A=D
D=M
@THIS
M=D      // THIS = *(FRAME-2)
@3
D=A
@Class1$FRAME.1
D=M-D
A=D
D=M
@ARG
M=D      // ARG = *(FRAME-3)
@4
D=A
@Class1$FRAME.1
D=M-D
A=D
D=M
@LCL
M=D      // LCL = *(FRAME-4)
@Class1$RET.1
A=M
0;JMP        // goto RET
(Class2.set)        // line = function Class2.set 0
@ARG      // line = push argument 0
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop static 0
M=M-1
@SP
A=M
D=M
@Class2.0
M=D
@ARG      // line = push argument 1
A=M
A=A+1
D=M
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop static 1
M=M-1
@SP
A=M
D=M
@Class2.1
M=D
@0     // line = push constant 0
D=A
@SP
A=M
M=D
@SP
M=M+1
@LCL        // line = return
D=M
@Class2$FRAME.2
M=D // FRAME = LCL
@5
D=A
@Class2$FRAME.2
D=M-D
A=D
D=M
@Class2$RET.2
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
@Class2$FRAME.2
D=M-D
A=D
D=M
@THAT
M=D      // THAT = *(FRAME-1)
@2
D=A
@Class2$FRAME.2
D=M-D
A=D
D=M
@THIS
M=D      // THIS = *(FRAME-2)
@3
D=A
@Class2$FRAME.2
D=M-D
A=D
D=M
@ARG
M=D      // ARG = *(FRAME-3)
@4
D=A
@Class2$FRAME.2
D=M-D
A=D
D=M
@LCL
M=D      // LCL = *(FRAME-4)
@Class2$RET.2
A=M
0;JMP        // goto RET
(Class2.get)        // line = function Class2.get 0
@Class2.0     // line = push static 0
D=M
@SP
A=M
M=D
@SP
M=M+1
@Class2.1     // line = push static 1
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
@Class2$FRAME.3
M=D // FRAME = LCL
@5
D=A
@Class2$FRAME.3
D=M-D
A=D
D=M
@Class2$RET.3
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
@Class2$FRAME.3
D=M-D
A=D
D=M
@THAT
M=D      // THAT = *(FRAME-1)
@2
D=A
@Class2$FRAME.3
D=M-D
A=D
D=M
@THIS
M=D      // THIS = *(FRAME-2)
@3
D=A
@Class2$FRAME.3
D=M-D
A=D
D=M
@ARG
M=D      // ARG = *(FRAME-3)
@4
D=A
@Class2$FRAME.3
D=M-D
A=D
D=M
@LCL
M=D      // LCL = *(FRAME-4)
@Class2$RET.3
A=M
0;JMP        // goto RET
(Sys.init)        // line = function Sys.init 0
@6     // line = push constant 6
D=A
@SP
A=M
M=D
@SP
M=M+1
@8     // line = push constant 8
D=A
@SP
A=M
M=D
@SP
M=M+1
@RET_ADDRESS.Class1.set     // line = call Class1.set 2
D=A
@SP
A=M
M=D
@SP
M=M+1        // push return-address
@LCL
D=M
@SP
A=M
M=D
@SP
M=M+1        // push LCL
@ARG
D=M
@SP
A=M
M=D
@SP
M=M+1        // push ARG
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THIS
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THAT
@SP
D=M
@2
D=D-A
@5
D=D-A
@ARG
M=D          // ARG=SP-n-5
@SP
D=M
@LCL
M=D          // LCL=SP
@Class1.set
0;JMP
(RET_ADDRESS.Class1.set)
@SP     // line = pop temp 0 
M=M-1
@SP
A=M
D=M
@5
M=D
@23     // line = push constant 23
D=A
@SP
A=M
M=D
@SP
M=M+1
@15     // line = push constant 15
D=A
@SP
A=M
M=D
@SP
M=M+1
@RET_ADDRESS.Class2.set     // line = call Class2.set 2
D=A
@SP
A=M
M=D
@SP
M=M+1        // push return-address
@LCL
D=M
@SP
A=M
M=D
@SP
M=M+1        // push LCL
@ARG
D=M
@SP
A=M
M=D
@SP
M=M+1        // push ARG
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THIS
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THAT
@SP
D=M
@2
D=D-A
@5
D=D-A
@ARG
M=D          // ARG=SP-n-5
@SP
D=M
@LCL
M=D          // LCL=SP
@Class2.set
0;JMP
(RET_ADDRESS.Class2.set)
@SP     // line = pop temp 0 
M=M-1
@SP
A=M
D=M
@5
M=D
@RET_ADDRESS.Class1.get     // line = call Class1.get 0
D=A
@SP
A=M
M=D
@SP
M=M+1        // push return-address
@LCL
D=M
@SP
A=M
M=D
@SP
M=M+1        // push LCL
@ARG
D=M
@SP
A=M
M=D
@SP
M=M+1        // push ARG
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THIS
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THAT
@SP
D=M
@0
D=D-A
@5
D=D-A
@ARG
M=D          // ARG=SP-n-5
@SP
D=M
@LCL
M=D          // LCL=SP
@Class1.get
0;JMP
(RET_ADDRESS.Class1.get)
@RET_ADDRESS.Class2.get     // line = call Class2.get 0
D=A
@SP
A=M
M=D
@SP
M=M+1        // push return-address
@LCL
D=M
@SP
A=M
M=D
@SP
M=M+1        // push LCL
@ARG
D=M
@SP
A=M
M=D
@SP
M=M+1        // push ARG
@THIS
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THIS
@THAT
D=M
@SP
A=M
M=D
@SP
M=M+1        // push THAT
@SP
D=M
@0
D=D-A
@5
D=D-A
@ARG
M=D          // ARG=SP-n-5
@SP
D=M
@LCL
M=D          // LCL=SP
@Class2.get
0;JMP
(RET_ADDRESS.Class2.get)
(WHILE)        // line = label WHILE
@WHILE     // line = goto WHILE
0;JMP