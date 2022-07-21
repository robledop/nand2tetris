// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed. 
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.

// Put your code here.

(KEYBOARD_INPUT)
    @KBD
    D=M
    @PRINT_BLACK
    D;JNE

(PRINT_WHITE)
    @SCREEN
    D=A
    @addr
    M=D                // addr = 16384
                       // (screen's base address)
    @8191
    D=A
    @n
    M=D               // n = RAM[0]

    @i
    M=0               // i = 0

(LOOP_WHITE)
    @i
    D=M
    @n
    D=D-M
    @KEYBOARD_INPUT
    D;JGT             // if i > n goto END

    @addr
    A=M
    M=0              // RAM[addr]=0

    @i
    M=M+1             // i = i + 1
    @1
    D=A
    @addr
    M=D+M             
    @LOOP_WHITE
    0;JMP             // goto LOOP_WHITE

(PRINT_BLACK)
    @SCREEN
    D=A
    @addr
    M=D                // addr = 16384
                       // (screen's base address)
    @8191
    D=A
    @n
    M=D               // n = RAM[0]

    @i
    M=0               // i = 0

(LOOP_BLACK)
    @i
    D=M
    @n
    D=D-M
    @KEYBOARD_INPUT
    D;JGT             // if i > n goto END

    @addr
    A=M
    M=-1              // RAM[addr]=1111111111111111

    @i
    M=M+1             // i = i + 1
    @1
    D=A
    @addr
    M=D+M             // addr = addr + 32
    @LOOP_BLACK
    0;JMP             // goto LOOP
