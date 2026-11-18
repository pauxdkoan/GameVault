/** Quita espacios repetidos y saltos de línea: "  Hola \n mundo " → "Hola mundo". */
export function normalizeText(value: string | null | undefined): string {
  return value?.replace(/\s+/g, ' ').trim() ?? '';
}

/** Quita vacíos y repetidos sin distinguir mayúsculas, conservando el primero que aparece. */
export function uniqueValues(values: string[]): string[] {
  return Array.from(
    new Map(
      values
        .map((value) => value.trim())
        .filter((value) => value !== '')
        .map((value) => [value.toLocaleLowerCase(), value]),
    ).values(),
  );
}
