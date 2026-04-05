using System;

namespace Nano.Data;

internal readonly record struct ForeignKeyMap(Type RootType, string[] PropertyNames);