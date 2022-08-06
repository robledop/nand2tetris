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
(Sys.init)        // line = function Sys.init 0
@4000     // line = push constant 4000	
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop pointer 0
M=M-1
@SP
A=M
D=M
@THIS
M=D
@SP
@5000     // line = push constant 5000
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop pointer 1
M=M-1
@SP
A=M
D=M
@THAT
M=D
@SP
@RET_ADDRESS.Sys.main     // line = call Sys.main 0
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
@Sys.main
0;JMP
(RET_ADDRESS.Sys.main)
@SP     // line = pop temp 1
M=M-1
@SP
A=M
D=M
@6
M=D
(LOOP)        // line = label LOOP
@LOOP     // line = goto LOOP
0;JMP
(Sys.main)        // line = function Sys.main 5
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
@SP
A=M
M=0
@SP
M=M+1
@4001     // line = push constant 4001
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop pointer 0
M=M-1
@SP
A=M
D=M
@THIS
M=D
@SP
@5001     // line = push constant 5001
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop pointer 1
M=M-1
@SP
A=M
D=M
@THAT
M=D
@SP
@200     // line = push constant 200
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop local 1
M=M-1
@SP
A=M
D=M
@LCL
A=M
A=A+1
M=D
@40     // line = push constant 40
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop local 2
M=M-1
@SP
A=M
D=M
@LCL
A=M
A=A+1
A=A+1
M=D
@6     // line = push constant 6
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop local 3
M=M-1
@SP
A=M
D=M
@LCL
A=M
A=A+1
A=A+1
A=A+1
M=D
@123     // line = push constant 123
D=A
@SP
A=M
M=D
@SP
M=M+1
@RET_ADDRESS.Sys.add12     // line = call Sys.add12 1
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
@1
D=D-A
@5
D=D-A
@ARG
M=D          // ARG=SP-n-5
@SP
D=M
@LCL
M=D          // LCL=SP
@Sys.add12
0;JMP
(RET_ADDRESS.Sys.add12)
@SP     // line = pop temp 0
M=M-1
@SP
A=M
D=M
@5
M=D
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
@LCL      // line = push local 2
A=M
A=A+1
A=A+1
D=M
@SP
A=M
M=D
@SP
M=M+1
@LCL      // line = push local 3
A=M
A=A+1
A=A+1
A=A+1
D=M
@SP
A=M
M=D
@SP
M=M+1
@LCL      // line = push local 4
A=M
A=A+1
A=A+1
A=A+1
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
@LCL        // line = return
D=M
@Sys$FRAME.0
M=D // FRAME = LCL
@5
D=A
@Sys$FRAME.0
D=M-D
A=D
D=M
@Sys$RET.0
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
@Sys$FRAME.0
D=M-D
A=D
D=M
@THAT
M=D      // THAT = *(FRAME-1)
@2
D=A
@Sys$FRAME.0
D=M-D
A=D
D=M
@THIS
M=D      // THIS = *(FRAME-2)
@3
D=A
@Sys$FRAME.0
D=M-D
A=D
D=M
@ARG
M=D      // ARG = *(FRAME-3)
@4
D=A
@Sys$FRAME.0
D=M-D
A=D
D=M
@LCL
M=D      // LCL = *(FRAME-4)
@Sys$RET.0
A=M
0;JMP        // goto RET
(Sys.add12)        // line = function Sys.add12 0
@4002     // line = push constant 4002
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop pointer 0
M=M-1
@SP
A=M
D=M
@THIS
M=D
@SP
@5002     // line = push constant 5002
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP     // line = pop pointer 1
M=M-1
@SP
A=M
D=M
@THAT
M=D
@SP
@ARG      // line = push argument 0
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1
@12     // line = push constant 12
D=A
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
@LCL        // line = return
D=M
@Sys$FRAME.1
M=D // FRAME = LCL
@5
D=A
@Sys$FRAME.1
D=M-D
A=D
D=M
@Sys$RET.1
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
@Sys$FRAME.1
D=M-D
A=D
D=M
@THAT
M=D      // THAT = *(FRAME-1)
@2
D=A
@Sys$FRAME.1
D=M-D
A=D
D=M
@THIS
M=D      // THIS = *(FRAME-2)
@3
D=A
@Sys$FRAME.1
D=M-D
A=D
D=M
@ARG
M=D      // ARG = *(FRAME-3)
@4
D=A
@Sys$FRAME.1
D=M-D
A=D
D=M
@LCL
M=D      // LCL = *(FRAME-4)
@Sys$RET.1
A=M
0;JMP        // goto RET