import { RouterOutlet } from '@angular/router';
import { PublicFooterComponent } from '../../shared/components/public-footer/public-footer';
import { PublicHeaderComponent } from '../../shared/components/public-header/public-header';
import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-public-layout',
  imports: [PublicHeaderComponent, PublicFooterComponent, RouterOutlet],
  templateUrl: './public-layout.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PublicLayout {}
