import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  ElementRef,
  HostListener,
  ViewChild,
  computed,
  inject,
  input,
  signal,
} from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faChevronLeft,
  faChevronRight,
  faCirclePlay,
  faXmark,
} from '@fortawesome/free-solid-svg-icons';
import { Video } from '../../models/games.interfaces';

interface SwiperElement extends HTMLElement {
  swiper?: {
    isBeginning: boolean;
    isEnd: boolean;
    slideNext: () => void;
    slidePrev: () => void;
    update: () => void;
  };
}

@Component({
  selector: 'app-game-details-media',
  imports: [FontAwesomeModule],
  templateUrl: './game-details-media.html',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GameDetailsMediaComponent implements AfterViewInit {
  readonly gameName = input.required<string>();
  readonly screenshots = input<string[]>([]);
  readonly videos = input<Video[]>([]);

  @ViewChild('imageScroller') private imageScroller?: ElementRef<HTMLElement>;
  @ViewChild('videoScroller') private videoScroller?: ElementRef<HTMLElement>;

  private readonly sanitizer = inject(DomSanitizer);

  protected readonly icons = {
    close: faXmark,
    next: faChevronRight,
    play: faCirclePlay,
    previous: faChevronLeft,
  };
  protected readonly selectedImageIndex = signal<number | null>(null);
  protected readonly selectedVideo = signal<Video | null>(null);
  protected readonly canScrollImagesPrevious = signal(false);
  protected readonly canScrollImagesNext = signal(false);
  protected readonly canScrollVideosPrevious = signal(false);
  protected readonly canScrollVideosNext = signal(false);
  protected readonly visibleScreenshots = computed(() =>
    this.screenshots().filter((screenshot) => screenshot.trim() !== ''),
  );
  protected readonly visibleVideos = computed(() =>
    this.videos().filter((video) => video.url || video.embedUrl),
  );
  protected readonly hasMedia = computed(
    () =>
      this.visibleScreenshots().length > 0 || this.visibleVideos().length > 0,
  );
  protected readonly selectedImage = computed(() => {
    const index = this.selectedImageIndex();

    return index === null ? null : (this.visibleScreenshots()[index] ?? null);
  });
  protected readonly selectedImageCounter = computed(() => {
    const index = this.selectedImageIndex();

    return index === null
      ? ''
      : `${index + 1} / ${this.visibleScreenshots().length}`;
  });
  protected readonly selectedVideoEmbedUrl = computed<SafeResourceUrl | null>(
    () => {
      const video = this.selectedVideo();
      const embedUrl = video?.embedUrl || video?.url;

      return embedUrl
        ? this.sanitizer.bypassSecurityTrustResourceUrl(embedUrl)
        : null;
    },
  );

  ngAfterViewInit(): void {
    queueMicrotask(() => {
      this.updateScrollControls(this.imageScroller?.nativeElement, 'images');
      this.updateScrollControls(this.videoScroller?.nativeElement, 'videos');
    });
  }

  @HostListener('document:keydown.escape')
  protected closeActiveViewer(): void {
    this.closeImageViewer();
    this.closeVideoViewer();
  }

  @HostListener('document:keydown.arrowleft')
  protected showPreviousFromKeyboard(): void {
    if (this.selectedImageIndex() !== null) {
      this.showPreviousImage();
    }
  }

  @HostListener('document:keydown.arrowright')
  protected showNextFromKeyboard(): void {
    if (this.selectedImageIndex() !== null) {
      this.showNextImage();
    }
  }

  protected scrollRow(
    scroller: HTMLElement,
    direction: -1 | 1,
    target: 'images' | 'videos',
  ): void {
    const swiper = this.getSwiper(scroller);

    if (direction === -1) {
      swiper?.slidePrev();
    } else {
      swiper?.slideNext();
    }

    this.updateScrollControls(scroller, target);
    window.setTimeout(() => this.updateScrollControls(scroller, target), 350);
  }

  protected updateScrollControls(
    scroller: HTMLElement | undefined,
    target: 'images' | 'videos',
  ): void {
    const swiper = scroller ? this.getSwiper(scroller) : null;
    swiper?.update();
    const canMovePrevious = swiper ? !swiper.isBeginning : false;
    const canMoveNext = swiper ? !swiper.isEnd : false;

    if (target === 'images') {
      this.canScrollImagesPrevious.set(canMovePrevious);
      this.canScrollImagesNext.set(canMoveNext);
      return;
    }

    this.canScrollVideosPrevious.set(canMovePrevious);
    this.canScrollVideosNext.set(canMoveNext);
  }

  protected openImageViewer(index: number): void {
    this.selectedImageIndex.set(index);
  }

  protected closeImageViewer(): void {
    this.selectedImageIndex.set(null);
  }

  protected showPreviousImage(): void {
    this.moveImageSelection(-1);
  }

  protected showNextImage(): void {
    this.moveImageSelection(1);
  }

  protected openVideoViewer(video: Video): void {
    this.selectedVideo.set(video);
  }

  protected closeVideoViewer(): void {
    this.selectedVideo.set(null);
  }

  private moveImageSelection(direction: -1 | 1): void {
    const screenshots = this.visibleScreenshots();
    const currentIndex = this.selectedImageIndex();

    if (screenshots.length === 0 || currentIndex === null) {
      return;
    }

    const nextIndex =
      (currentIndex + direction + screenshots.length) % screenshots.length;

    this.selectedImageIndex.set(nextIndex);
  }

  private getSwiper(scroller: HTMLElement): SwiperElement['swiper'] | null {
    return (scroller as SwiperElement).swiper ?? null;
  }
}
