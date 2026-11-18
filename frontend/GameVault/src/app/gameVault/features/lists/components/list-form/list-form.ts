import { ChangeDetectionStrategy, Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import { errorMessages } from '../../../../core/http/api-error';
import { VaultButton } from '../../../../shared/ui/button/vault-button';
import { GameListSummary, ListRequest } from '../../models/list.models';
import { ListsService } from '../../services/lists.service';

const MAX_NAME = 80;
const MAX_DESCRIPTION = 500;

/** Formulario para crear una lista o editar nombre, descripción y visibilidad. */
@Component({
  selector: 'app-list-form',
  imports: [VaultButton],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // Los componentes de Angular son "inline" por defecto; block hace que respeten márgenes y space-y.
  host: { class: 'block' },
  templateUrl: './list-form.html',
})
export class ListForm implements OnInit {
  private readonly listsService = inject(ListsService);

  /** Con un id edita esa lista; sin él crea una nueva. */
  readonly listId = input<string | null>(null);
  readonly initial = input<ListRequest | null>(null);
  readonly saved = output<GameListSummary>();
  readonly cancelled = output<void>();

  protected readonly name = signal('');
  protected readonly description = signal('');
  protected readonly isPublic = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly submitted = signal(false);
  protected readonly errors = signal<string[]>([]);
  protected readonly maxName = MAX_NAME;
  protected readonly maxDescription = MAX_DESCRIPTION;

  protected readonly validationError = computed(() => {
    if (!this.name().trim()) return 'Ponle un nombre a la lista.';
    if (this.name().length > MAX_NAME) return `El nombre no puede superar los ${MAX_NAME} caracteres.`;
    if (this.description().length > MAX_DESCRIPTION) return `La descripción no puede superar los ${MAX_DESCRIPTION} caracteres.`;
    return null;
  });

  ngOnInit(): void {
    const initial = this.initial();
    if (initial) {
      this.name.set(initial.name);
      this.description.set(initial.description ?? '');
      this.isPublic.set(initial.isPublic);
    }
  }

  protected textValue(event: Event): string {
    return (event.target as HTMLInputElement | HTMLTextAreaElement).value;
  }

  protected submit(event: Event): void {
    event.preventDefault();
    this.submitted.set(true);
    if (this.validationError()) return;

    const request: ListRequest = {
      name: this.name().trim(),
      description: this.description().trim() || null,
      isPublic: this.isPublic(),
    };
    const listId = this.listId();
    const save$ = listId ? this.listsService.update(listId, request) : this.listsService.create(request);

    this.isSaving.set(true);
    this.errors.set([]);
    save$.subscribe({
      next: (list) => {
        this.isSaving.set(false);
        this.saved.emit(list);
      },
      error: (error: unknown) => {
        this.isSaving.set(false);
        this.errors.set(errorMessages(error));
      },
    });
  }
}
