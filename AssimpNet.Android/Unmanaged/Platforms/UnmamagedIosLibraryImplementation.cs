﻿/*
* Copyright (c) 2012-2020 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Runtime.InteropServices;

namespace Assimp.Unmanaged
{
    internal sealed class UnmanagedIosLibraryImplementation : UnmanagedLibraryImplementation
    {
        public override String DllExtension
        {
            get
            {
                return ""; // not used
            }
        }

        public override String DllPrefix
        {
            get
            {
                return ""; // not used
            }
        }

        public UnmanagedIosLibraryImplementation(String defaultLibName, Type[] unmanagedFunctionDelegateTypes)
            : base(defaultLibName, unmanagedFunctionDelegateTypes)
        {
        }

        protected override IntPtr NativeLoadLibrary(String path)
        {
            IntPtr libraryHandle = dlopen(null, RTLD_NOW);

            if (libraryHandle == IntPtr.Zero && ThrowOnLoadFailure)
            {
                IntPtr errPtr = dlerror();
                String msg = Marshal.PtrToStringAnsi(errPtr);
                if (!String.IsNullOrEmpty(msg))
                    throw new AssimpException(String.Format("Error loading unmanaged library\n\n{0}", msg));
                else
                    throw new AssimpException(String.Format("Error loading unmanaged library"));
            }

            return libraryHandle;
        }

        protected override IntPtr NativeGetProcAddress(IntPtr handle, String functionName)
        {
            return dlsym(handle, functionName);
        }

        protected override void NativeFreeLibrary(IntPtr handle)
        {
            dlclose(handle);
        }

        #region Native Methods

        [DllImport("libSystem.B.dylib")]
        private static extern IntPtr dlopen(String fileName, int flags);

        [DllImport("libSystem.B.dylib")]
        private static extern IntPtr dlsym(IntPtr handle, String functionName);

        [DllImport("libSystem.B.dylib")]
        private static extern int dlclose(IntPtr handle);

        [DllImport("libSystem.B.dylib")]
        private static extern IntPtr dlerror();

        private const int RTLD_NOW = 2;

        #endregion
    }
}
