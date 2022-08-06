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
(Main.fibonacci)        // line = function Main.fibonacci 0
@ARG      // line = push argument 0
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1
@2     // line = push constant 2
D=A
@SP
A=M
M=D
@SP
M=M+1
@SP      // line = lt
M=M-1
A=M
D=M
@SP
M=M-1
A=M
D=M-D
M=-1
@LT.TRUE.0
D;JLT
@SP
A=M
M=0
(LT.TRUE.0)
@SP
A=M
@SP
M=M+1
@SP     // line = if-goto IF_TRUE
AM=M-1
D=M
@IF_TRUE
D;JNE
@IF_FALSE     // line = goto IF_FALSE
0;JMP
(IF_TRUE)        // line = label IF_TRUE          
@ARG      // line = push argument 0        
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1
@LCL        // line = return
D=M
@Main$FRAME.0
M=D // FRAME = LCL
@5
D=A
@Main$FRAME.0
D=M-D
A=D
D=M
@Main$RET.0
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
@Main$FRAME.0
D=M-D
A=D
D=M
@THAT
M=D      // THAT = *(FRAME-1)
@2
D=A
@Main$FRAME.0
D=M-D
A=D
D=M
@THIS
M=D      // THIS = *(FRAME-2)
@3
D=A
@Main$FRAME.0
D=M-D
A=D
D=M
@ARG
M=D      // ARG = *(FRAME-3)
@4
D=A
@Main$FRAME.0
D=M-D
A=D
D=M
@LCL
M=D      // LCL = *(FRAME-4)
@Main$RET.0
A=M
0;JMP        // goto RET
(IF_FALSE)        // line = label IF_FALSE         
@ARG      // line = push argument 0
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1
@2     // line = push constant 2
D=A
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
@RET_ADDRESS.Main.fibonacci     // line = call Main.fibonacci 1  
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
@Main.fibonacci
0;JMP
(RET_ADDRESS.Main.fibonacci)
@ARG      // line = push argument 0
A=M
D=M
@SP
A=M
M=D
@SP
M=M+1
@1     // line = push constant 1
D=A
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
@RET_ADDRESS.Main.fibonacci     // line = call Main.fibonacci 1  
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
@Main.fibonacci
0;JMP
(RET_ADDRESS.Main.fibonacci)
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
@Main$FRAME.1
M=D // FRAME = LCL
@5
D=A
@Main$FRAME.1
D=M-D
A=D
D=M
@Main$RET.1
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
@Main$FRAME.1
D=M-D
A=D
D=M
@THAT
M=D      // THAT = *(FRAME-1)
@2
D=A
@Main$FRAME.1
D=M-D
A=D
D=M
@THIS
M=D      // THIS = *(FRAME-2)
@3
D=A
@Main$FRAME.1
D=M-D
A=D
D=M
@ARG
M=D      // ARG = *(FRAME-3)
@4
D=A
@Main$FRAME.1
D=M-D
A=D
D=M
@LCL
M=D      // LCL = *(FRAME-4)
@Main$RET.1
A=M
0;JMP        // goto RET
(Sys.init)        // line = function Sys.init 0
@4     // line = push constant 4
D=A
@SP
A=M
M=D
@SP
M=M+1
@RET_ADDRESS.Main.fibonacci     // line = call Main.fibonacci 1   
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
@Main.fibonacci
0;JMP
(RET_ADDRESS.Main.fibonacci)
(WHILE)        // line = label WHILE
@WHILE     // line = goto WHILE              
0;JMP