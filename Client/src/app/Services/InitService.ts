import { inject, Injectable } from "@angular/core";
import { MatIconRegistry } from "@angular/material/icon";
import { DomSanitizer } from "@angular/platform-browser";

@Injectable({ providedIn: 'root' })
export class InitService {
    readonly iconRegistry = inject(MatIconRegistry);
    readonly sanitizer = inject(DomSanitizer);

    /**
     * Register application SVG icons at startup.
     * Uses the `public` folder (mapped to /) so icons live under `/icons/...`.
     */
    init(): void {
        const icons = [
            { name: 'dashboard', url: '/icons/dashboard.svg' },
            { name: 'games', url: '/icons/games.svg' },
            { name: 'add', url: '/icons/add.svg' },
            { name: 'edit', url: '/icons/edit.svg' },
            { name: 'delete', url: '/icons/delete.svg' }
        ];

        for (const icon of icons) {
            this.iconRegistry.addSvgIcon(icon.name, this.sanitizer.bypassSecurityTrustResourceUrl(icon.url));
        }
    }
}