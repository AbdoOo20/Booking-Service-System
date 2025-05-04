# Sales Tax Calculator - Solution Documentation

## Fixed Issues

### 1. Input Parsing Issues
- **Problem**: Original parser failed with multi-word product names
- **Fix**: Improved word splitting logic and "at" keyword detection
- **Changes**:
  - Now correctly handles items like "1 box of imported chocolates"
  - Better error handling for malformed inputs

### 2. Tax Calculation Errors
- **Problem**: Incorrect tax rates for imported exempt items
- **Fix**: Implemented proper tax categories:
  ```csharp
  if (isImported && IsTaxExempt) // 5% only
  else if (!IsTaxExempt) // 10% (15% if imported)

### 3. Enhanced Tax Exemption Detection
- **New Method**: Added dedicated `IsTaxExempt` method
- **Features**:
  ```csharp
  private bool IsTaxExempt(string productName)
  {
      string lowerName = productName.ToLower();
      return lowerName.Contains("book") ||
             lowerName.Contains("chocolate") ||
             lowerName.Contains("chips") ||
             lowerName.Contains("food") ||
             lowerName.Contains("pill") ||
             lowerName.Contains("tablet") ||
             lowerName.Contains("medicine");
  }
