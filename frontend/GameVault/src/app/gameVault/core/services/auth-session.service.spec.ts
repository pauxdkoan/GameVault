import { TestBed } from '@angular/core/testing';
import { LoginResponse, SessionUser } from '../../features/auth/models/login.interfaces';
import { AuthSessionService } from './auth-session.service';

const user: SessionUser = {
  id: 'u1',
  email: 'ana@example.com',
  userName: 'ana',
  displayName: 'Ana Ruiz',
  bio: null,
  avatarUrl: null,
  bannerUrl: null,
  country: null,
  isActive: true,
};

const login: LoginResponse = {
  accessToken: 'access-1',
  refreshToken: 'refresh-1',
  accessTokenExpiresAt: '2030-01-01T00:00:00Z',
  user,
};

describe('AuthSessionService', () => {
  /** Crea el servicio de nuevo, como si se recargara la página. */
  function createService(): AuthSessionService {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({});
    return TestBed.inject(AuthSessionService);
  }

  beforeEach(() => localStorage.clear());
  afterEach(() => localStorage.clear());

  it('empieza sin sesión', () => {
    const session = createService();

    expect(session.isLoggedIn()).toBeFalse();
    expect(session.user()).toBeNull();
    expect(session.accessToken()).toBeNull();
  });

  it('start() guarda tokens y usuario', () => {
    const session = createService();
    session.start(login);

    expect(session.isLoggedIn()).toBeTrue();
    expect(session.accessToken()).toBe('access-1');
    expect(session.refreshToken()).toBe('refresh-1');
    expect(session.user()?.userName).toBe('ana');
  });

  it('recuerda la sesión al recargar la página', () => {
    createService().start(login);

    const reloaded = createService();

    expect(reloaded.isLoggedIn()).toBeTrue();
    expect(reloaded.user()?.displayName).toBe('Ana Ruiz');
  });

  it('calcula las iniciales a partir del nombre visible', () => {
    const session = createService();
    session.start(login);

    expect(session.initials()).toBe('AR');
  });

  it('updateUser() cambia el usuario sin tocar los tokens', () => {
    const session = createService();
    session.start(login);
    session.updateUser({ ...user, displayName: 'Ana R.' });

    expect(session.user()?.displayName).toBe('Ana R.');
    expect(session.accessToken()).toBe('access-1');
    expect(createService().user()?.displayName).toBe('Ana R.');
  });

  it('clear() borra la sesión también de localStorage', () => {
    const session = createService();
    session.start(login);
    session.clear();

    expect(session.isLoggedIn()).toBeFalse();
    expect(session.user()).toBeNull();
    expect(createService().isLoggedIn()).toBeFalse();
  });

  it('ignora un usuario guardado que no es JSON válido', () => {
    localStorage.setItem('gv.user', '{roto');

    expect(createService().user()).toBeNull();
  });
});
