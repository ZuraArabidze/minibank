using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Common;

public class DomainException(string message) : Exception(message);