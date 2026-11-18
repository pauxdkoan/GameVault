export interface RegisterRequest {
  name: string;
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface RegisterResponse {
  UserName: string;
  DisplayName: string;
  Bio: string | null;
  AvatarUrl: string | null;
  BannerUrl: string | null;
  Country: string | null;
  IsActive: boolean;
}
