namespace Application.Common;

public record PageResult<T>(int Total, IReadOnlyList<T> Data);
