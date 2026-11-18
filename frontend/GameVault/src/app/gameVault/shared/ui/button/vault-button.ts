import { Component, computed, input } from '@angular/core';

export type VaultButtonVariant = 'primary' | 'secondary' | 'ghost';
export type VaultButtonSize = 'sm' | 'md' | 'lg';

/**
 * Botón reutilizable de GameVault.
 *
 * Se aplica como atributo sobre un <button> o un <a>, así conserva
 * todo el comportamiento nativo (submit, routerLink, disabled...).
 *
 * Uso:
 *   <button vaultButton type="submit">Entrar</button>
 *   <a vaultButton variant="secondary" routerLink="/games">Ver juegos</a>
 */
@Component({
  selector: 'button[vaultButton], a[vaultButton]',
  template: '<ng-content />',
  host: {
    '[class]': 'classes()',
  },
})
export class VaultButton {
  readonly variant = input<VaultButtonVariant>('primary');
  readonly size = input<VaultButtonSize>('md');
  readonly fullWidth = input(false);

  private readonly baseClasses =
    'inline-flex items-center justify-center gap-2 rounded-md font-semibold transition duration-150 active:scale-[0.98] ' +
    'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-vault-accent focus-visible:ring-offset-2 focus-visible:ring-offset-vault-bg ' +
    'disabled:cursor-not-allowed disabled:opacity-60 cursor-pointer select-none';

  private readonly variantClasses: Record<VaultButtonVariant, string> = {
    primary:
      'bg-vault-accent text-vault-bg hover:bg-vault-accent-soft',
    secondary:
      'border border-vault-border-strong bg-vault-surface text-vault-text hover:border-vault-muted hover:bg-vault-surface-hover',
    ghost: 'text-vault-muted hover:bg-vault-surface-hover hover:text-vault-text',
  };

  private readonly sizeClasses: Record<VaultButtonSize, string> = {
    sm: 'h-9 px-3.5 text-sm',
    md: 'h-11 px-5 text-sm',
    lg: 'h-12 px-6 text-base',
  };

  protected readonly classes = computed(() =>
    [
      this.baseClasses,
      this.variantClasses[this.variant()],
      this.sizeClasses[this.size()],
      this.fullWidth() ? 'w-full' : '',
    ].join(' '),
  );
}
