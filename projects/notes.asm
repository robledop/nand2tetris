SP 	 	0
LCL  	1
ARG  	2
THIS 	3
THAT  	4


@10		// Load the A register with constant 10
D=M 	// Load the D register with the value at RAM[10]

@x 		// Load the A register with the address for the variable x (starts at 16)
D=M 	// Load the D register with the value at the address x

@y		// Load the A register with the address for the variable y
M=D 	// Set the value of y to the contents of the D register

// FUNCTION
// #####################################################################
// (f)
//     repeat k times:
//     PUSH 0


// RETURN
// #####################################################################
// FRAME = LCL
@LCL    // Load the A register with the address for the label LCL
D=M 	// Load the D register with the value at the address LCL
@FRAME  // Load the A register with the address for the variable FRAME
M=D 	// Set the value of FRAME to the contents of the D register

// RET = *(FRAME-5)
@5 		// Load the A register with the address for the constant 5
D=A 	// Load the D register with the value 5
@FRAME  // Load the A register with the address for the variable FRAME
D=M-D 	// Load the D register with the value at the address FRAME minus 5
@RET 	// Load the A register with the address for the variable RET
M=D 	// Set the value of RET to the contents of the D register

// *ARG = pop()
@SP 	// Load the A register with the address for the stack pointer
M=M-1 	// Decrement the stack pointer by 1
A=M 	// Load the A register with the value at the address SP
D=M 	// Load the D register with the value at the address RAM[SP]
@ARG 	// Load the A register with the address for the label ARG
M=D 	// Set the value of RAM[ARG] to the contents of the D register

// SP = ARG + 1
@ARG 	// Load the A register with the address for the label ARG
D=M 	// Load the D register with the value at RAM[ARG]
@SP 	// Load the A register with the address for the stack pointer
M=D+1 	// Set the value of RAM[SP] to the value of ARG plus 1

// THAT = *(FRAME-1)
@1 		// Load the A register with the address for the constant 1
D=A 	// Load the D register with the value 1
@FRAME  // Load the A register with the address for the variable FRAME
D=M-D 	// Load the D register with the value at the address FRAME minus 1
@THAT 	// Load the A register with the address for the variable THAT
M=D 	// Set the value of RAM[THAT] to the contents of the D register

// THIS = *(FRAME-2)
@2 		// Load the A register with the address for the constant 2
D=A 	// Load the D register with the value 2
@FRAME  // Load the A register with the address for the variable FRAME
D=M-D 	// Load the D register with the value at RAM[FRAME] minus 2
@THIS 	// Load the A register with the address for the label THIS
M=D 	// Set the value of RAM[THIS] to the contents of the D register

// ARG = *(FRAME-3)
@3 		// Load the A register with the address for the constant 3
D=A 	// Load the D register with the value 3
@FRAME  // Load the A register with the address for the variable FRAME
D=M-D 	// Load the D register with the value at RAM[FRAME] minus 3
@ARG 	// Load the A register with the address for the label ARG
M=D 	// Set the value of RAM[ARG] to the contents of the D register

// LCL = *(FRAME-4)
@4 		// Load the A register with the address for the constant 4
D=A 	// Load the D register with the value 4
@FRAME  // Load the A register with the address for the variable FRAME
D=M-D 	// Load the D register with the value at RAM[FRAME] minus 4
@LCL 	// Load the A register with the address for the label LCL
M=D 	// Set the value of RAM[LCL] to the contents of the D register

// goto RET
@RET 	// Load the A register with the address for the label RET
A=M 	// Load the A register with the value at RAM[RET]
0;JMP 	// Jump to the address in the A register

// #####################################################################









// PSEUDO CODE
    // x=R1
    // y=R2
    // R3=0
    // while(x > 0) {
    //     R3 += y
    //     x--
    // }
    
    @R1     // Load the A register with the address of R1
    D=M     // D = RAM[A]
    @x      // Load the A register with the address of x
    M=D     // RAM[x] = D (x = R1)

    @R2     // Load the A register with the address of R2
    D=M     // D = RAM[A]
    @y      // Load the A register with the address of y
    M=D     // y = R2

    @0      // Load the A register with the constant 0
    D=A     // D = 0
    @R3     // Load the A register with the address of R3
    M=D     // R3 = 0

(WHILE)
    // begin of loop condition
    @x          // Load the A register with the address of x
    D=M         // D = RAM[x]
    @END        // Load the A register with the address of the end of the loop
    D;JLE       // if x <= 0 proceed to END      
    // end of loop condition

    @y          // Load the A register with the address of y
    D=M         // D = y
    @R3         // Load the A register with the address of R3
    M=D+M       // RAM[A] = y + RAM[A] 
    @1
    D=A         // D = 1
    @x          // Load the A register with the address of x
    M=M-D       // RAM[x] = RAM[x] - 1


    @WHILE      // Load the A register with the address of the beginning of the loop
    0;JMP       // Jump to the beginning of the loop
(END)   
    @END        // Load the A register with the address of the end of the loop
    0;JMP       // Jump to the end of the loop