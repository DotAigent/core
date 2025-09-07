#!/bin/sh

# Output-filens namn
OUTPUT_FILE="all_cs_code.txt"

# Töm output-filen om den redan finns
> "$OUTPUT_FILE"

# Hitta alla .cs-filer rekursivt och loopa igenom dem
find . -type f -name "*.cs" | while read -r file; do
  if [ -f "$file" ]; then  # Kontrollera att det är en fil
    echo "===== START OF FILE: $file =====" >> "$OUTPUT_FILE"
    cat "$file" >> "$OUTPUT_FILE"
    echo "===== END OF FILE: $file =====" >> "$OUTPUT_FILE"
    echo "" >> "$OUTPUT_FILE"  # Tom rad för separation
  fi
done

echo "Alla C#-filer (inklusive underkataloger) har kopierats till $OUTPUT_FILE."
