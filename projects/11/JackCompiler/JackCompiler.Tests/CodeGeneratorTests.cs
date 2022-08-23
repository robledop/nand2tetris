﻿using System.Xml;
using FluentAssertions;
using JackCompiler.JackAnalyzer;
using JackCompiler.JackCodeGenerator;

namespace JackCompiler.Tests;

public class CodeGeneratorTests
{


	[Fact]
	public void Square_GenerateClassSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Square.jack");
		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);

		codeGenerator.GenerateClassSymbolTable();

		codeGenerator.ClassSymbolTable.Count.Should().Be(3);

		var compareTo = new List<Symbol>
		{
			new Symbol("x", "int", SymbolKind.Field, 0),
			new Symbol("y", "int", SymbolKind.Field, 1),
			new Symbol("size", "int", SymbolKind.Field, 2),
		};

		codeGenerator.ClassSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void Square_GenerateSubroutineSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Square.jack");
		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var constructor = parseTree.SelectSingleNode("//subroutineDec[identifier=' new ']");

		codeGenerator.CompileSubroutine(constructor);


		var compareTo = new List<Symbol>
		{
			new Symbol("Ax", "int", SymbolKind.Argument, 0),
			new Symbol("Ay", "int", SymbolKind.Argument, 1),
			new Symbol("Asize", "int", SymbolKind.Argument, 2)
		};

		codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void SquareMain_GenerateClassSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Main.jack");

		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.GenerateClassSymbolTable();

		codeGenerator.ClassSymbolTable.Count.Should().Be(1);

		var compareTo = new List<Symbol>
		{
			new Symbol("test", "boolean", SymbolKind.Static, 0)
		};

		codeGenerator.ClassSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void SquareMain_GenerateSubroutineSymbolTable_more()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Main.jack");
		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);

		var functionMore = parseTree.SelectSingleNode("//subroutineDec[identifier=' more ']");

		codeGenerator.CompileSubroutine(functionMore);

		var compareTo = new List<Symbol>
		{
			new Symbol("i", "int", SymbolKind.Local, 0),
			new Symbol("j", "int", SymbolKind.Local, 1),
			new Symbol("s", "String", SymbolKind.Local, 2),
			new Symbol("a", "Array", SymbolKind.Local, 3),
		};

		codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void Square_GenerateSubroutineSymbolTable_draw()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\Square\", "Square.jack");
		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);

		var functionMore = parseTree.SelectSingleNode("//subroutineDec[identifier=' draw ']");
		codeGenerator.CompileClass();
		codeGenerator.CompileSubroutine(functionMore);

		codeGenerator.SubroutineSymbolTable.Count.Should().Be(1);

		var compareTo = new List<Symbol>
		{
			new Symbol("this", "Square", SymbolKind.Argument, 0),
		};

		codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void ArrayTestMain_GenerateClassSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");

		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);

		codeGenerator.GenerateClassSymbolTable();

		codeGenerator.ClassSymbolTable.Count.Should().Be(0);
	}

	[Fact]
	public void ArrayTestMain_GenerateSubroutineSymbolTable()
	{
		string path = Path.Combine(Environment.CurrentDirectory, @"TestSource\ArrayTest\", "Main.jack");

		var source = File.ReadAllText(path);

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var functionMain = parseTree.SelectSingleNode("//subroutineDec[identifier=' main ']");

		codeGenerator.CompileSubroutine(functionMain);

		var compareTo = new List<Symbol>
		{
			new Symbol("a", "Array", SymbolKind.Local, 0),
			new Symbol("length", "int", SymbolKind.Local, 1),
			new Symbol("i", "int", SymbolKind.Local, 2),
			new Symbol("sum", "int", SymbolKind.Local, 3),
		};

		codeGenerator.SubroutineSymbolTable.Should().BeEquivalentTo(compareTo);
	}

	[Fact]
	public void CompileExpression_IntegerConstant()
	{
		var expressionNodeXml = @"
			<expression>
				<term>
					<integerConstant> 10 </integerConstant>
				</term>
			</expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml);

		var codeGenerator = new CodeGenerator(xmlDocument);

		var vmCode = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode.Should().Be("push constant 10");
	}

	[Fact]
	public void CompileExpression_Argument()
	{
		var source = @"
			class Square {
				field int x, y, size;
			   constructor Square new(int Ax, int Ay, int Asize) {
				  let x = Ax;
				  let y = Ay;
				  let size = Asize;
				  return this;
			   }
			}
			";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
		   <expression>
			<term>
			  <identifier> Ax </identifier>
			</term>
		  </expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be("push argument 0");

		var expressionNodeXml2 = @"
			<expression>
				<term>
					<identifier> Ay </identifier>
				</term>
			</expression>";
		xmlDocument.LoadXml(expressionNodeXml2);

		var vmCode2 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode2.Should().Be("push argument 1");
	}

	[Fact]
	public void CompileExpression_UnaryOp()
	{
		var source = @"
			class Main {
				function void more() { 
					var int i, j;      
					let i = i * (-j);
					return;
				}
			}
			";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
		   <expression>
				<term>
					<symbol> - </symbol>
					<term>
						<identifier> j </identifier>
					</term>
				</term>
		   </expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode.Should().Be(
@"push local 1
neg");
	}

	[Fact]
	public void CompileExpression_Op()
	{
		var source = @"
			class Main {
				function void more() { 
					var int i, j;      
					let i = i | j;
					let i = i * j;
					return;
				}
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
			<expression>
				<term>
					<identifier> i </identifier>
				</term>
				<symbol> | </symbol>
				<term>
					<identifier> j </identifier>
				</term>
			</expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
@"push local 0
push local 1
or");

		var expressionNodeXml2 = @"
			<expression>
				<term>
					<identifier> i </identifier>
				</term>
				<symbol> * </symbol>
				<term>
					<identifier> j </identifier>
				</term>
			</expression>";

		xmlDocument.LoadXml(expressionNodeXml2);
		var vmCode2 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();
		vmCode2.Should().Be(
@"push local 0
push local 1
call Math.multiply 2");
	}

	[Fact]
	public void CompileExpression_ExpressionListEmpty()
	{
		var source = @"
			class Cell 
			{
				field Array _neighbors;

				method void determineNextLiveState()
				{
					var Cell currentNeighbor;
					var int liveNeighbors, i;
					
					let liveNeighbors = 0;
					let i = 0;
					while(i < 8)
					{
						let currentNeighbor = _neighbors[i];
						if(currentNeighbor.getIsAlive())
						{
							let liveNeighbors = liveNeighbors + 1;
						}
						let i = i + 1;
					}
				}
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
				<expression>
					<term>
					  <identifier> currentNeighbor </identifier>
					  <symbol> . </symbol>
					  <identifier> getIsAlive </identifier>
					  <symbol> ( </symbol>
					  <expressionList>
					  </expressionList>
					  <symbol> ) </symbol>
					</term>
				</expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
@"push local 0
call Cell.getIsAlive 0");
	}

	[Fact]
	public void CompileExpression_ExpressionList()
	{
		var source = @"
			class Cell 
			{
				method void determineNextLiveState()
				{
					var Cell currentNeighbor;
					var int a;
					let a = 10;
					if(currentNeighbor.getIsAlive(1, a))
					{
					}
				}
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var expressionNodeXml1 = @"
				<expression>
					<term>
					  <identifier> currentNeighbor </identifier>
					  <symbol> . </symbol>
					  <identifier> getIsAlive </identifier>
					  <symbol> ( </symbol>
					  <expressionList>
						  <expression>
							<term>
								<integerConstant> 1 </integerConstant>
							</term>
						  </expression>
						  <symbol> , </symbol>
						  <expression>
							<term>
								<identifier> a </identifier>
							</term>
						  </expression>
					  </expressionList>
					  <symbol> ) </symbol>
					</term>
				</expression>";

		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(expressionNodeXml1);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileExpression(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
@"push local 0
push constant 1
push local 1
call Cell.getIsAlive 2");
	}

	[Fact]
	public void CompileLetStatements()
	{
		var source = @"
			class Cell {
			  method void determineNextLiveState() {
				var Cell currentNeighbor;
				var int liveNeighbors, i;
				let liveNeighbors = 0;
				let i = 0;
				return;
			  }
			}
";
		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var statementNodeXml = @"
		<statements>
			<letStatement>
			  <keyword> let </keyword>
			  <identifier> liveNeighbors </identifier>
			  <symbol> = </symbol>
			  <expression>
				<term>
				  <integerConstant> 0 </integerConstant>
				</term>
			  </expression>
			  <symbol> ; </symbol>
			</letStatement>
			<letStatement>
			  <keyword> let </keyword>
			  <identifier> i </identifier>
			  <symbol> = </symbol>
			  <expression>
				<term>
				  <integerConstant> 0 </integerConstant>
				</term>
			  </expression>
			  <symbol> ; </symbol>
			</letStatement>
		</statements>
";
		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(statementNodeXml);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileStatements(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
			@"push constant 0
pop local 1
push constant 0
pop local 2");
	}

	[Fact]
	public void CompileWhileStatements()
	{
		var source = @"
			class Cell {
			   method void determineNextLiveState() {
				var int liveNeighbors, i;
					
				let i = 0;
				while(i < 8)
				{
				  let i = i + 1;
				}

				return;
			  }
			}
";
		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var statementNodeXml = @"
		<statements>
			<whileStatement>
			  <keyword> while </keyword>
			  <symbol> ( </symbol>
			  <expression>
				<term>
				  <identifier> i </identifier>
				</term>
				<symbol> &lt; </symbol>
				<term>
				  <integerConstant> 8 </integerConstant>
				</term>
			  </expression>
			  <symbol> ) </symbol>
			  <symbol> { </symbol>
			  <statements>
				<letStatement>
				  <keyword> let </keyword>
				  <identifier> i </identifier>
				  <symbol> = </symbol>
				  <expression>
					<term>
					  <identifier> i </identifier>
					</term>
					<symbol> + </symbol>
					<term>
					  <integerConstant> 1 </integerConstant>
					</term>
				  </expression>
				  <symbol> ; </symbol>
				</letStatement>
			  </statements>
			  <symbol> } </symbol>
			</whileStatement>
		</statements>
";
		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(statementNodeXml);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileStatements(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
			@"label WHILE_EXP1
push local 1
push constant 8
lt
not
if-goto WHILE_END1
push local 1
push constant 1
add
pop local 1
goto WHILE_EXP1
label WHILE_END1");
	}

	[Fact]
	public void CompileIfStatements()
	{
		var source = @"
			class Cell {
			   method void determineNextLiveState() {
				var int liveNeighbors, i;
					
				let i = 0;
				while(i < 8)
				{
				  if ( i < 4 ) {
					let i = i + 1;
				  } else {
					let i = 1 + 1;
				  }
				  let i = i + 1;
				}

				return;
			  }
			}
";
		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var statementNodeXml = @"
		<statements>
			<ifStatement>
			  <keyword> if </keyword>
			  <symbol> ( </symbol>
			  <expression>
				<term>
				  <identifier> i </identifier>
				</term>
				<symbol> &lt; </symbol>
				<term>
				  <integerConstant> 8 </integerConstant>
				</term>
			  </expression>
			  <symbol> ) </symbol>
			  <symbol> { </symbol>
			  <statements>
				<letStatement>
				  <keyword> let </keyword>
				  <identifier> i </identifier>
				  <symbol> = </symbol>
				  <expression>
					<term>
					  <identifier> i </identifier>
					</term>
					<symbol> + </symbol>
					<term>
					  <integerConstant> 1 </integerConstant>
					</term>
				  </expression>
				  <symbol> ; </symbol>
				</letStatement>
			  </statements>
			  <symbol> } </symbol>
			  <keyword> else </keyword>
			  <symbol> { </symbol>
			  <statements>
				<letStatement>
				  <keyword> let </keyword>
				  <identifier> i </identifier>
				  <symbol> = </symbol>
				  <expression>
					<term>
					  <identifier> i </identifier>
					</term>
					<symbol> + </symbol>
					<term>
					  <integerConstant> 1 </integerConstant>
					</term>
				  </expression>
				  <symbol> ; </symbol>
				</letStatement>
			  </statements>
			  <symbol> } </symbol>
			</ifStatement>
		</statements>
";
		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(statementNodeXml);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileStatements(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
			@"push local 1
push constant 8
lt
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push local 1
push constant 1
add
pop local 1
goto IF_END1
label IF_FALSE1
push local 1
push constant 1
add
pop local 1
label IF_END1");
	}

	[Fact]
	public void CompileDoStatements()
	{
		var source = @"
			class Cell {
			  field Array _neighbors;
			  field Cell _cell;

			  method void test()   {
				do Memory.deAlloc(_neighbors);
				do _cell.advance(1);
				return;
			  }
			}
";
		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();

		var statementNodeXml = @"
		<statements>
			<doStatement>
			  <keyword> do </keyword>
			  <identifier> Memory </identifier>
			  <symbol> . </symbol>
			  <identifier> deAlloc </identifier>
			  <symbol> ( </symbol>
			  <expressionList>
				<expression>
				  <term>
					<identifier> _neighbors </identifier>
				  </term>
				</expression>
			  </expressionList>
			  <symbol> ) </symbol>
			  <symbol> ; </symbol>
			</doStatement>
			<doStatement>
			  <keyword> do </keyword>
			  <identifier> _cell </identifier>
			  <symbol> . </symbol>
			  <identifier> advance </identifier>
			  <symbol> ( </symbol>
			  <expressionList>
				<expression>
				  <term>
					<integerConstant> 1 </integerConstant>
				  </term>
				</expression>
			  </expressionList>
			  <symbol> ) </symbol>
			  <symbol> ; </symbol>
			</doStatement>
		</statements>
";
		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(statementNodeXml);

		var codeGenerator = new CodeGenerator(parseTree);
		codeGenerator.CompileClass();

		var vmCode1 = codeGenerator.CompileStatements(xmlDocument.FirstChild).Trim();

		vmCode1.Should().Be(
@"push this 0
call Memory.deAlloc 1
pop temp 0
push this 1
push constant 1
call Cell.advance 1
pop temp 0");
	}

	[Fact]
	public void CompileSeven()
	{
		var source = @"
			class Main {
			   function void main() {
				  do Output.printInt(1 + (2 * 3));
				  return;
			   }
			}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
@"function Main.main 0
push constant 1
push constant 2
push constant 3
call Math.multiply 2
add
call Output.printInt 1
pop temp 0
push constant 0
return
");
	}

	[Fact]
	public void CompileConvertToBin()
	{
		var source = @"
		   class Main {
			function void main() {
				var int value;
				do Main.fillMemory(8001, 16, -1); // sets RAM[8001]..RAM[8016] to -1
				let value = Memory.peek(8000);    // reads a value from RAM[8000]
				do Main.convert(value);           // performs the conversion
				return;
			}
			
			function void convert(int value) {
				var int mask, position;
				var boolean loop;
				
				let loop = true;
				while (loop) {
					let position = position + 1;
					let mask = Main.nextMask(mask);
				
					if (~(position > 16)) {
				
						if (~((value & mask) = 0)) {
							do Memory.poke(8000 + position, 1);
						}
						else {
							do Memory.poke(8000 + position, 0);
						}    
					}
					else {
						let loop = false;
					}
				}
				return;
			}
		 
			function int nextMask(int mask) {
				if (mask = 0) {
					return 1;
				}
				else {
				return mask * 2;
				}
			}
			
			function void fillMemory(int startAddress, int length, int value) {
				while (length > 0) {
					do Memory.poke(startAddress, value);
					let length = length - 1;
					let startAddress = startAddress + 1;
				}
				return;
			}
		}";

		var parser = new Parser();
		parser.GetTokens(source);
		var parseTree = parser.ParseClass();
		var codeGenerator = new CodeGenerator(parseTree);
		var vmCode = codeGenerator.CompileClass();

		vmCode.Should().Be(
@"function Main.main 1
push constant 8001
push constant 16
push constant 1
neg
call Main.fillMemory 3
pop temp 0
push constant 8000
call Memory.peek 1
pop local 0
push local 0
call Main.convert 1
pop temp 0
push constant 0
return
function Main.convert 3
push constant 0
not
pop local 2
label WHILE_EXP0
push local 2
not
if-goto WHILE_END0
push local 1
push constant 1
add
pop local 1
push local 0
call Main.nextMask 1
pop local 0
push local 1
push constant 16
gt
not
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push argument 0
push local 0
and
push constant 0
eq
not
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push constant 8000
push local 1
add
push constant 1
call Memory.poke 2
pop temp 0
goto IF_END1
label IF_FALSE1
push constant 8000
push local 1
add
push constant 0
call Memory.poke 2
pop temp 0
label IF_END1
goto IF_END0
label IF_FALSE0
push constant 0
pop local 2
label IF_END0
goto WHILE_EXP0
label WHILE_END0
push constant 0
return
function Main.nextMask 0
push argument 0
push constant 0
eq
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push constant 1
return
goto IF_END2
label IF_FALSE2
push argument 0
push constant 2
call Math.multiply 2
return
label IF_END2
function Main.fillMemory 0
label WHILE_EXP1
push argument 1
push constant 0
gt
not
if-goto WHILE_END1
push argument 0
push argument 2
call Memory.poke 2
pop temp 0
push argument 1
push constant 1
sub
pop argument 1
push argument 0
push constant 1
add
pop argument 0
goto WHILE_EXP1
label WHILE_END1
push constant 0
return
");
	}
}