using System;
using System.Runtime.InteropServices;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Collections.Generic;

namespace XLY.SF.Project.Domains
{
    public abstract class SQLiteFtsTokenizer
    {
        /// <summary>
        /// Name of tokenizer. (Better to override this to avoid conflicts)
        /// </summary>
        public virtual string Name
        {
            get { return "custom"; }
        }

        /// <summary>
        /// Register this tokenizer to SQLite connection.
        /// </summary>
        /// <param name="connection">opened connection.</param>
        public void RegisterMe(SQLiteConnection connection)
        {
            internalArea = new InternalArea(this);
            IntPtr ppModule = internalArea.CreateModule();
            int size = Marshal.SizeOf(ppModule);
            byte[] bytes = new byte[size];
            if (size == 4)
            {
                bytes = BitConverter.GetBytes(ppModule.ToInt32());
            }
            else if (size == 8)
            {
                bytes = BitConverter.GetBytes(ppModule.ToInt64());
            }

            SQLiteCommand command = new SQLiteCommand("SELECT fts3_tokenizer(?, ?)", connection);
            SQLiteParameter p0 = new SQLiteParameter();
            p0.DbType = DbType.String;
            p0.Value = this.Name;
            command.Parameters.Add(p0);
            SQLiteParameter p1 = new SQLiteParameter();
            p1.DbType = DbType.Binary;
            p1.Size = size;
            p1.Value = bytes;
            command.Parameters.Add(p1);
            SQLiteDataReader dr = command.ExecuteReader();
            dr.Close();
            connection.Disposed += delegate { internalArea.DisposeModule(); };
        }

        private InternalArea internalArea;

        #region InternalArea
        private class InternalArea
        {
            #region Sqlite3 FTS3 Tokenizer Interface
            /// Return Type: int
            ///argc: int
            ///argv: char**
            ///ppTokenizer: sqlite3_tokenizer**
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int sqlite3_tokenizer_module_xCreate(int argc, ref IntPtr argv, ref IntPtr ppTokenizer);

            /// Return Type: int
            ///pTokenizer: sqlite3_tokenizer*
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int sqlite3_tokenizer_module_xDestroy(ref sqlite3_tokenizer pTokenizer);

            /// Return Type: int
            ///pTokenizer: sqlite3_tokenizer*
            ///pInput: char*
            ///nBytes: int
            ///ppCursor: sqlite3_tokenizer_cursor**
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int sqlite3_tokenizer_module_xOpen(ref sqlite3_tokenizer pTokenizer,
                IntPtr pInput,
                int nBytes,
                ref IntPtr ppCursor);

            /// Return Type: int
            ///pCursor: sqlite3_tokenizer_cursor*
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int sqlite3_tokenizer_module_xClose(ref sqlite3_tokenizer_cursor pCursor);

            /// Return Type: int
            ///pCursor: sqlite3_tokenizer_cursor*
            ///ppToken: char**
            ///pnBytes: int*
            ///piStartOffset: int*
            ///piEndOffset: int*
            ///piPosition: int*
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int sqlite3_tokenizer_module_xNext(ref sqlite3_tokenizer_cursor pCursor,
                ref IntPtr ppToken, ref int pnBytes, ref int piStartOffset, ref int piEndOffset, ref int piPosition);

            [StructLayoutAttribute(LayoutKind.Sequential)]
            internal struct sqlite3_tokenizer_module
            {
                /// int
                public int iVersion;

                /// sqlite3_tokenizer_module_xCreate
                public sqlite3_tokenizer_module_xCreate xCreate;

                /// sqlite3_tokenizer_module_xDestroy
                public sqlite3_tokenizer_module_xDestroy xDestroy;

                /// sqlite3_tokenizer_module_xOpen
                public sqlite3_tokenizer_module_xOpen xOpen;

                /// sqlite3_tokenizer_module_xClose
                public sqlite3_tokenizer_module_xClose xClose;

                /// sqlite3_tokenizer_module_xNext
                public sqlite3_tokenizer_module_xNext xNext;
            }

            [StructLayoutAttribute(LayoutKind.Sequential)]
            internal struct sqlite3_tokenizer
            {
                /// sqlite3_tokenizer_module*
                public IntPtr pModule;
            }

            [StructLayoutAttribute(LayoutKind.Sequential)]
            internal struct sqlite3_tokenizer_cursor
            {
                /// sqlite3_tokenizer*
                public IntPtr pTokenizer;
            }
            #endregion

            private SQLiteFtsTokenizer owner;
            private sqlite3_tokenizer_module module;
            private IntPtr ppTokenizer;
            private IntPtr ppCursor;
            private IntPtr ppModule;
            private const int SQLITE_OK = 0;
            private const int SQLITE_ERROR = 1;
            private const int SQLITE_DONE = 101;

            public InternalArea(SQLiteFtsTokenizer owner)
            {
                this.owner = owner;
            }

            private IntPtr MarshalStruct(object obj)
            {
                IntPtr address = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
                Marshal.StructureToPtr(obj, address, false);
                return address;
            }

            public IntPtr CreateModule()
            {
                this.module = new sqlite3_tokenizer_module();
                this.module.xCreate = this.xCreate;
                this.module.xDestroy = this.xDestroy;
                this.module.xOpen = this.xOpen;
                this.module.xClose = this.xClose;
                this.module.xNext = this.xNext;
                this.ppModule = this.MarshalStruct(module);
                return this.ppModule;
            }

            public void DisposeModule()
            {
                Marshal.FreeHGlobal(ppModule);
            }

            private int xCreate(int argc, ref IntPtr argv, ref IntPtr ppTokenizer)
            {
                try
                {
                    sqlite3_tokenizer pTokenizer = new sqlite3_tokenizer();
                    this.ppTokenizer = this.MarshalStruct(pTokenizer);
                    ppTokenizer = this.ppTokenizer;
                    string tokenizerArgument = null;
                    if (argc > 0)
                    {
                        tokenizerArgument = SQLiteConvert.UTF8ToString(argv, -1);
                    }
                    owner.OnCreate(tokenizerArgument);
                    return SQLITE_OK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return SQLITE_ERROR;
                }
            }

            private int xDestroy(ref sqlite3_tokenizer pTokenizer)
            {
                owner.OnDestroy();
                Marshal.FreeHGlobal(this.ppTokenizer);
                return SQLITE_OK;
            }

            private int xOpen(ref sqlite3_tokenizer pTokenizer,
                IntPtr pInput,
                int nBytes,
                ref IntPtr ppCursor)
            {
                try
                {
                    sqlite3_tokenizer_cursor pCursor = new sqlite3_tokenizer_cursor();
                    pCursor.pTokenizer = this.ppTokenizer;
                    this.ppCursor = this.MarshalStruct(pCursor);
                    ppCursor = this.ppCursor;

                    owner.inputString = SQLiteConvert.UTF8ToString(pInput, nBytes);
                    owner.tokenNumber = -1;

                    owner.PrepareToStart();
                    return SQLITE_OK;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return SQLITE_ERROR;
                }
            }

            private int xClose(ref sqlite3_tokenizer_cursor pCursor)
            {
                if (this.ppCursor != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(this.ppCursor);
                    this.ppCursor = IntPtr.Zero;
                }
                return SQLITE_OK;
            }

            private IntPtr lastStringPtr = IntPtr.Zero;

            private int xNext(ref sqlite3_tokenizer_cursor pCursor,
                ref IntPtr ppToken, ref int pnBytes, ref int piStartOffset, ref int piEndOffset, ref int piPosition)
            {
                if (this.lastStringPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(this.lastStringPtr);
                    this.lastStringPtr = IntPtr.Zero;
                }

                if (owner.MoveNext())
                {
                    byte[] bytes = SQLiteConvert.ToUTF8(owner.Token);
                    pnBytes = bytes.Length;

                    this.lastStringPtr = Marshal.AllocHGlobal(pnBytes);
                    Marshal.Copy(bytes, 0, this.lastStringPtr, pnBytes);
                    ppToken = this.lastStringPtr;
                    pnBytes--;

                    string prevString = owner.InputString.Substring(0, owner.TokenIndexOfString);
                    piStartOffset = Encoding.UTF8.GetByteCount(prevString);
                    if (owner.NextIndexOfString < 0) // default
                    {
                        piEndOffset = piStartOffset + pnBytes;
                    }
                    else
                    {
                        prevString = owner.InputString.Substring(0, owner.NextIndexOfString);
                        piEndOffset = Encoding.UTF8.GetByteCount(prevString);
                    }
                    owner.tokenNumber++;
                    piPosition = owner.tokenNumber;

                    return SQLITE_OK;
                }
                else
                {
                    return SQLITE_DONE;
                }
            }
        }
        #endregion

        /// <summary>
        /// When tokenizer inits itself.
        /// </summary>
        /// <param name="tokenizerArgument">The argument for tokenizer.</param>
        protected virtual void OnCreate(string tokenizerArgument) { }

        /// <summary>
        /// When tokenizer destroy itself.
        /// </summary>
        protected virtual void OnDestroy() { }

        /// <summary>
        /// Before tokenizer starts to work.
        /// </summary>
        protected abstract void PrepareToStart();

        /// <summary>
        /// Try to find next token. Return false if no token found.
        /// </summary>
        /// <returns>Return true if you found a token. Return false if no token found.</returns>
        protected abstract bool MoveNext();

        private string inputString;
        private string token;
        private int tokenIndexOfString;
        private int nextIndexOfString = -1;
        private int tokenNumber = -1;

        /// <summary>
        /// The input string to be tokenized.
        /// </summary>
        protected string InputString
        {
            get { return this.inputString; }
        }
        /// <summary>
        /// The token you found.
        /// </summary>
        protected string Token
        {
            get { return this.token; }
            set { this.token = value; }
        }
        /// <summary>
        /// The index of the input string where you found the token.
        /// </summary>
        protected int TokenIndexOfString
        {
            get { return this.tokenIndexOfString; }
            set { this.tokenIndexOfString = value; }
        }
        /// <summary>
        /// The index of the input string where you start next finding. You may set it -1 if next finding will start just after the current token.
        /// </summary>
        protected int NextIndexOfString
        {
            get { return this.nextIndexOfString; }
            set { this.nextIndexOfString = value; }
        }

        /// <summary>
        /// While developing, you may use this method to test your tokenizing code, without connecting to SQLite database.
        /// </summary>
        /// <param name="inputString">The input string for tokenize.</param>
        /// <returns>A List contains the founded tokens.</returns>
        public List<string> TestMe(string inputString)
        {
            return TestMe(inputString, null);
        }

        /// <summary>
        /// While developing, you may use this method to test your tokenizing code, without connecting to SQLite database.
        /// </summary>
        /// <param name="inputString">The input string for tokenize.</param>
        /// <param name="tokenizerArgument">Argument of tokenizer.</param>
        /// <returns>A List contains the founded tokens.</returns>
        public List<string> TestMe(string inputString, string tokenizerArgument)
        {
            OnCreate(tokenizerArgument);
            this.inputString = inputString;
            PrepareToStart();
            List<string> tokens = new List<string>();
            while (this.MoveNext())
            {
                tokens.Add(this.Token);
            }
            OnDestroy();
            return tokens;
        }
    }
}
