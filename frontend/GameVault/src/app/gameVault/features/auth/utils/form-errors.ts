import { AbstractControl } from '@angular/forms';

/**
 * Mensajes de error por tipo de validación.
 * La clave es el nombre del error de Angular (required, email, minlength...).
 */
export type ErrorMessages = Partial<Record<string, string>>;

/**
 * Devuelve el primer mensaje de error de un control,
 * pero solo si el usuario ya interactuó con él (touched).
 * Así los templates quedan limpios: [error]="fieldError('email')"
 */
export function getControlError(
  control: AbstractControl | null,
  messages: ErrorMessages,
): string | null {
  if (!control || !control.errors || !control.touched) {
    return null;
  }

  const firstErrorKey = Object.keys(control.errors).find(
    (key) => messages[key],
  );

  return firstErrorKey ? messages[firstErrorKey]! : null;
}
