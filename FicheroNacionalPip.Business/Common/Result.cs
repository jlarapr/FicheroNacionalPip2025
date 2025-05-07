namespace FicheroNacionalPip.Business.Common;

/// <summary>
/// Representa el resultado de una operación que puede ser exitosa con un valor de tipo TSuccess
/// o fallida con un error de tipo TFailure.
/// </summary>
/// <typeparam name="TSuccess">El tipo del valor en caso de éxito</typeparam>
/// <typeparam name="TFailure">El tipo del error en caso de fallo</typeparam>
public abstract class Result<TSuccess, TFailure>
{
    /// <summary>
    /// Representa un resultado exitoso que contiene un valor.
    /// </summary>
    public sealed class Success : Result<TSuccess, TFailure>
    {
        /// <summary>
        /// Obtiene el valor del resultado exitoso.
        /// </summary>
        public TSuccess Value { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Success.
        /// </summary>
        /// <param name="value">El valor del resultado exitoso.</param>
        public Success(TSuccess value) => Value = value;
    }

    /// <summary>
    /// Representa un resultado fallido que contiene un error.
    /// </summary>
    public sealed class Failure : Result<TSuccess, TFailure>
    {
        /// <summary>
        /// Obtiene el error del resultado fallido.
        /// </summary>
        public TFailure Error { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase Failure.
        /// </summary>
        /// <param name="error">El error del resultado fallido.</param>
        public Failure(TFailure error) => Error = error;
    }

    /// <summary>
    /// Obtiene un valor que indica si el resultado es exitoso.
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    /// Obtiene un valor que indica si el resultado es fallido.
    /// </summary>
    public bool IsFailure => this is Failure;

    /// <summary>
    /// Obtiene el valor del resultado si es exitoso, o el valor predeterminado si es fallido.
    /// </summary>
    /// <returns>El valor del resultado o el valor predeterminado.</returns>
    public TSuccess? GetValueOrDefault()
        => this is Success s ? s.Value : default;

    /// <summary>
    /// Obtiene el error del resultado si es fallido, o el valor predeterminado si es exitoso.
    /// </summary>
    /// <returns>El error del resultado o el valor predeterminado.</returns>
    public TFailure? GetErrorOrDefault()
        => this is Failure f ? f.Error : default;

    /// <summary>
    /// Transforma el valor de un resultado exitoso usando una función de mapeo.
    /// </summary>
    /// <typeparam name="TNew">El nuevo tipo del valor exitoso.</typeparam>
    /// <param name="mapper">La función de transformación a aplicar al valor.</param>
    /// <returns>Un nuevo resultado con el valor transformado o el error original.</returns>
    public Result<TNew, TFailure> Map<TNew>(Func<TSuccess, TNew> mapper)
        => this is Success s
            ? new Result<TNew, TFailure>.Success(mapper(s.Value))
            : new Result<TNew, TFailure>.Failure(((Failure)this).Error);

    /// <summary>
    /// Transforma un resultado en otro resultado usando una función de mapeo.
    /// </summary>
    /// <typeparam name="TNew">El nuevo tipo del valor exitoso.</typeparam>
    /// <param name="binder">La función que transforma el valor en un nuevo resultado.</param>
    /// <returns>El nuevo resultado.</returns>
    public Result<TNew, TFailure> Bind<TNew>(Func<TSuccess, Result<TNew, TFailure>> binder)
        => this is Success s ? binder(s.Value) : new Result<TNew, TFailure>.Failure(((Failure)this).Error);

    /// <summary>
    /// Obtiene el valor del resultado si es exitoso, o lanza una excepción si es fallido.
    /// </summary>
    /// <param name="message">El mensaje de error opcional para la excepción.</param>
    /// <returns>El valor del resultado.</returns>
    /// <exception cref="InvalidOperationException">Se lanza cuando el resultado es fallido.</exception>
    public TSuccess GetValueOrThrow(string? message = null)
        => this is Success s
            ? s.Value
            : throw new InvalidOperationException(message ?? $"El resultado no contiene un valor. Error: {GetErrorOrDefault()}");

    /// <summary>
    /// Ejecuta una acción si el resultado es exitoso.
    /// </summary>
    /// <param name="action">La acción a ejecutar con el valor exitoso.</param>
    /// <returns>El mismo resultado para permitir encadenamiento.</returns>
    public Result<TSuccess, TFailure> OnSuccess(Action<TSuccess> action)
    {
        if (this is Success s)
            action(s.Value);
        return this;
    }

    /// <summary>
    /// Ejecuta una acción si el resultado es fallido.
    /// </summary>
    /// <param name="action">La acción a ejecutar con el error.</param>
    /// <returns>El mismo resultado para permitir encadenamiento.</returns>
    public Result<TSuccess, TFailure> OnFailure(Action<TFailure> action)
    {
        if (this is Failure f)
            action(f.Error);
        return this;
    }

    /// <summary>
    /// Combina dos resultados en uno solo que contiene una tupla con ambos valores.
    /// </summary>
    /// <typeparam name="T1">Tipo del primer valor exitoso.</typeparam>
    /// <typeparam name="T2">Tipo del segundo valor exitoso.</typeparam>
    /// <typeparam name="TError">Tipo del error.</typeparam>
    /// <param name="first">Primer resultado a combinar.</param>
    /// <param name="second">Segundo resultado a combinar.</param>
    /// <returns>Un resultado combinado que es exitoso solo si ambos resultados son exitosos.</returns>
    public static Result<(T1, T2), TError> Combine<T1, T2, TError>(
        Result<T1, TError> first,
        Result<T2, TError> second)
    {
        if (first is Result<T1, TError>.Success s1 && second is Result<T2, TError>.Success s2)
            return new Result<(T1, T2), TError>.Success((s1.Value, s2.Value));

        var error = first is Result<T1, TError>.Failure f1 
            ? f1.Error 
            : ((Result<T2, TError>.Failure)second).Error;

        return new Result<(T1, T2), TError>.Failure(error);
    }

    /// <summary>
    /// Crea un resultado exitoso con el valor especificado.
    /// </summary>
    /// <param name="value">El valor para el resultado exitoso.</param>
    /// <returns>Un nuevo resultado exitoso.</returns>
    public static Result<TSuccess, TFailure> Ok(TSuccess value)
        => new Success(value);

    /// <summary>
    /// Crea un resultado fallido con el error especificado.
    /// </summary>
    /// <param name="error">El error para el resultado fallido.</param>
    /// <returns>Un nuevo resultado fallido.</returns>
    public static Result<TSuccess, TFailure> Fail(TFailure error)
        => new Failure(error);

    /// <summary>
    /// Aplica una de dos funciones dependiendo del estado del resultado.
    /// </summary>
    /// <typeparam name="TResult">El tipo del resultado de la transformación.</typeparam>
    /// <param name="onSuccess">Función a aplicar si el resultado es exitoso.</param>
    /// <param name="onFailure">Función a aplicar si el resultado es fallido.</param>
    /// <returns>El resultado de aplicar la función correspondiente.</returns>
    public TResult Match<TResult>(
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure)
        => this is Success s ? onSuccess(s.Value) : onFailure(((Failure)this).Error);

    /// <summary>
    /// Ejecuta una acción con el valor exitoso sin modificar el resultado.
    /// Similar a OnSuccess pero con un nombre más funcional.
    /// </summary>
    /// <param name="action">La acción a ejecutar con el valor exitoso.</param>
    /// <returns>El mismo resultado para permitir encadenamiento.</returns>
    public Result<TSuccess, TFailure> Tap(Action<TSuccess> action)
        => OnSuccess(action);

    /// <summary>
    /// Ejecuta una acción con el error sin modificar el resultado.
    /// Similar a OnFailure pero con un nombre más funcional.
    /// </summary>
    /// <param name="action">La acción a ejecutar con el error.</param>
    /// <returns>El mismo resultado para permitir encadenamiento.</returns>
    public Result<TSuccess, TFailure> TapError(Action<TFailure> action)
        => OnFailure(action);

    /// <summary>
    /// Ejecuta una acción independientemente del estado del resultado.
    /// </summary>
    /// <param name="action">La acción a ejecutar.</param>
    /// <returns>El mismo resultado para permitir encadenamiento.</returns>
    public Result<TSuccess, TFailure> TapBoth(Action<TSuccess> onSuccess, Action<TFailure> onFailure)
    {
        if (this is Success s)
            onSuccess(s.Value);
        else
            onFailure(((Failure)this).Error);
        return this;
    }
} 