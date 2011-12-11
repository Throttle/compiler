using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;

namespace CoolCore.Compilation
{
    public class Generator
    {
        private Module m_Module = null;
        private SymbolTable m_Globals = null;

        public Generator(Module module)
        {
            m_Module = module;
        }

        public void Compile(string path)
        {
            AppDomain domain = System.Threading.Thread.GetDomain();
            AssemblyName name = new AssemblyName();
            name.Name = "Sharp Code Assembly";

            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(m_Module.Name, m_Module.Name + ".exe", true);

            //
            // Create global variables.
            //

            TypeBuilder globalBuilder = moduleBuilder.DefineType("Global");
            m_Globals = new SymbolTable();
            foreach (Statement statement in m_Module.Body.Statements)
            {
                if (statement is Variable)
                {
                    Variable variable = statement as Variable;
                    System.Type type = variable.Type.ToSystemType();
                    FieldBuilder field = globalBuilder.DefineField(variable.Name, type, FieldAttributes.Public | FieldAttributes.Static);
                    m_Globals.Add(variable.Name, SymbolType.Variable, statement, field);
                }
            }

            globalBuilder.CreateType();

            //
            // Create functions.
            //

            m_Module.Body.SymbolTable = m_Globals;
            BuildFunctionStubs(m_Module.Body, moduleBuilder);

            foreach (Function function in m_Module.Body.Functions)
            {
                BuildFunction(function);
            }

            //
            // Create entry point.
            //

            MethodBuilder mainBuilder = moduleBuilder.DefineGlobalMethod("Main", MethodAttributes.Public | MethodAttributes.Static, typeof(void), null);
            ILGenerator mainIL = mainBuilder.GetILGenerator();

            EmitBody(mainIL, m_Module.Body, true);

            moduleBuilder.CreateGlobalFunctions();
            assemblyBuilder.SetEntryPoint(mainBuilder.GetBaseDefinition());
            assemblyBuilder.Save(m_Module.Name + ".exe");

            System.IO.File.Move(m_Module.Name + ".exe", path);

        }
    }
}
