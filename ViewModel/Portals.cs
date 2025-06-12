using Core.Language;
using Action = System.Action;

namespace ViewModel;

public static class Portals
{
    public static (Func, Type[]) Portal(this Func<int, string, int> call)
    {
        var @params = call.Method.GetParameters();
        var types = new Type[@params.Length];
        for (int i = 0; i < @params.Length; i++)
        {
            types[i] = @params[i].ParameterType;
        }

        // otra forma de convertir de ParamsInfo a Type
        // types = @params.Select(x => x.ParameterType).ToArray();

        object PortalFunc(params object?[] @params)
        {
            return call((int)@params[0]!, (string)@params[1]!);
        }
        return (PortalFunc, types);
    }

    public static Func Portal<T1, T2, T3>( Func<T1, T2, T3> call) => 
        (@params) =>
    {
        if (@params.Length != 2)
            throw new Exception();
        if (@params[0] is not T1 param1)
            throw new Exception();
        if (@params[1] is not T2 param2)
            throw new Exception();
        return call(param1, param2)!;
    };

    public static Func Portal<T1>(Func<T1> call) =>
       (@params) =>
       {
           if (@params.Length != 0)
               throw new Exception();
           return call()!;
       };

    public static Func Portal<T1, T2>(Func<T1, T2> call) =>
       (@params) =>
       {
           if (@params.Length != 1)
               throw new Exception();
           if (@params[0] is not T1 param1)
               throw new Exception();
           return call(param1)!;
       };

    public static Func Portal<T1, T2, T3, T4>(Func<T1, T2, T3, T4> call) =>
        (@params) =>
        {
            if (@params.Length != 3)
                throw new Exception();
            if (@params[0] is not T1 param1)
                throw new Exception();
            if (@params[1] is not T2 param2)
                throw new Exception();
            if (@params[2] is not T3 param3)
                throw new Exception();
            return call(param1, param2, param3)!;
        };

    public static Func Portal<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> call) =>
        (@params) =>
        {
            if (@params.Length != 4)
                throw new Exception();
            if (@params[0] is not T1 param1)
                throw new Exception();
            if (@params[1] is not T2 param2)
                throw new Exception();
            if (@params[2] is not T3 param3)
                throw new Exception();
            if (@params[3] is not T4 param4)
                throw new Exception();
            return call(param1, param2, param3, param4)!;
        };

    public static Func Portal<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> call) =>
        (@params) =>
        {
            if (@params.Length != 5)
                throw new Exception();
            if (@params[0] is not T1 param1)
                throw new Exception();
            if (@params[1] is not T2 param2)
                throw new Exception();
            if (@params[2] is not T3 param3)
                throw new Exception();
            if (@params[3] is not T4 param4)
                throw new Exception();
            if (@params[4] is not T5 param5)
                throw new Exception();
            return call(param1, param2, param3, param4, param5)!;
        };

    public static Core.Language.Action Portal<T1, T2>(Action<T1, T2> call) =>
      (@params) =>
      {
          if (@params.Length != 2)
              throw new Exception();
          if (@params[0] is not T1 param1)
              throw new Exception();
          if (@params[1] is not T2 param2)
              throw new Exception();
          call(param1, param2);
      };

    public static Core.Language.Action Portal<T1>(Action<T1> call) =>
      (@params) =>
      {
          if (@params.Length != 1)
              throw new Exception();
          if (@params[0] is not T1 param1)
              throw new Exception();
          call(param1);
      };

    public static Core.Language.Action Portal(Action call) =>
      (@params) =>
      {
          if (@params.Length != 0)
              throw new Exception();
          call();
      };

    public static Core.Language.Action Portal<T1, T2, T3>(Action<T1, T2, T3> call) =>
      (@params) =>
      {
          if (@params.Length != 3)
              throw new Exception();
          if (@params[0] is not T1 param1)
              throw new Exception();
          if (@params[1] is not T2 param2)
              throw new Exception();
          if (@params[2] is not T3 param3)
              throw new Exception();
          call(param1, param2, param3);
      };

    public static Core.Language.Action Portal<T1, T2, T3, T4>(Action<T1, T2, T3, T4> call) =>
      (@params) =>
      {
          if (@params.Length != 4)
              throw new Exception();
          if (@params[0] is not T1 param1)
              throw new Exception();
          if (@params[1] is not T2 param2)
              throw new Exception();
          if (@params[2] is not T3 param3)
              throw new Exception();
          if (@params[3] is not T4 param4)
              throw new Exception();
          call(param1, param2, param3, param4);
      };

    public static Core.Language.Action Portal<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> call) =>
     (@params) =>
     {
         if (@params.Length != 5)
             throw new Exception();
         if (@params[0] is not T1 param1)
             throw new Exception();
         if (@params[1] is not T2 param2)
             throw new Exception();
         if (@params[2] is not T3 param3)
             throw new Exception();
         if (@params[3] is not T4 param4)
             throw new Exception();
         if (@params[4] is not T5 param5)
             throw new Exception();
         call(param1, param2, param3, param4, param5);
     };
}