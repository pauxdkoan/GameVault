import { GameCardDto } from '../../../shared/models/game-card.model';
import { UserSummary } from '../../../shared/models/api.models';

export interface GameListSummary {
  id: string;
  name: string;
  description: string | null;
  isPublic: boolean;
  gameCount: number;
  coverImages: string[];
  owner: UserSummary;
  createdAt: string;
  updatedAt: string | null;
}

export interface GameListItem {
  position: number;
  note: string | null;
  game: GameCardDto;
}

export interface GameListDetail {
  id: string;
  name: string;
  description: string | null;
  isPublic: boolean;
  owner: UserSummary;
  isMine: boolean;
  items: GameListItem[];
  createdAt: string;
  updatedAt: string | null;
}

export interface ListRequest {
  name: string;
  description: string | null;
  isPublic: boolean;
}
