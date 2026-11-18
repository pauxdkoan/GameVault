import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

/** Foto del usuario o, si no tiene, sus iniciales sobre un color fijo para ese nombre. */
@Component({
  selector: 'app-user-avatar',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (avatarUrl()) {
      <img
        class="shrink-0 rounded-full object-cover"
        [style.width.px]="size()"
        [style.height.px]="size()"
        [src]="avatarUrl()"
        [alt]="name()"
      />
    } @else {
      <span
        class="grid shrink-0 place-items-center rounded-full font-bold"
        [class]="colorClass()"
        [style.width.px]="size()"
        [style.height.px]="size()"
        [style.font-size.px]="size() * 0.38"
        aria-hidden="true"
      >
        {{ initials() }}
      </span>
    }
  `,
})
export class UserAvatar {
  readonly name = input.required<string>();
  readonly avatarUrl = input<string | null>(null);
  readonly size = input(40);

  protected readonly initials = computed(() =>
    this.name()
      .split(/\s+/)
      .filter(Boolean)
      .slice(0, 2)
      .map((part) => part.charAt(0).toUpperCase())
      .join(''),
  );

  /** El mismo nombre siempre da el mismo color. */
  protected readonly colorClass = computed(() => {
    const colors = ['bg-vault-accent/15 text-vault-accent', 'bg-vault-surface-hover text-vault-text', 'bg-vault-success/15 text-vault-success', 'bg-vault-warning/15 text-vault-warning'];
    const hash = [...this.name()].reduce((total, char) => total + char.charCodeAt(0), 0);
    return colors[hash % colors.length];
  });
}
