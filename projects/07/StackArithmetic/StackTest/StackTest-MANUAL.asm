// push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// eq
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
D=M-D
@EQ.TRUE.1
D;JEQ
@SP
A=M
M=0
(EQ.TRUE.1)
@SP
A=M
M=1
@SP
M=M+1

// push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 16
@16
D=A
@SP
A=M
M=D
@SP
M=M+1

// eq
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
D=M-D
@EQ.TRUE.2
D;JEQ
@SP
A=M
M=0
(EQ.TRUE.2)
@SP
A=M
M=1
@SP
M=M+1

// push constant 16
@16
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1

// eq
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
D=M-D
@EQ.TRUE.3
D;JEQ
@SP
A=M
M=0
(EQ.TRUE.3)
@SP
A=M
M=1
@SP
M=M+1

// push constant 892
@892
D=A
@SP
A=M
M=D
@SP
M=M+1

// push constant 891
@891
D=A
@SP
A=M
M=D
@SP
M=M+1

// lt
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
D=M-D
@EQ.TRUE.3
D;JLT
@SP
A=M
M=0
(EQ.TRUE.3)
@SP
A=M
M=1
@SP
M=M+1

// and
@SP
M=M-1
A=M
D=M
@SP
M=M-1
A=M
M=D&M
@SP
M=M+1



