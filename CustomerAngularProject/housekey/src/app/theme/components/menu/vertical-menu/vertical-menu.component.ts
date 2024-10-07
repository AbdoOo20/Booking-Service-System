import { Component, OnInit, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { Menu } from '@models/menu.model';
import { TranslateModule } from '@ngx-translate/core';
import { MenuService } from '@services/menu.service';

@Component({
  selector: 'app-vertical-menu',
  standalone: true,
  imports: [
    RouterModule,
    TranslateModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './vertical-menu.component.html',
  styleUrls: ['./vertical-menu.component.scss'],
  providers: [MenuService]
})
export class VerticalMenuComponent implements OnInit {
  @Input('menuParentId') menuParentId: number;
  public menuItems: Array<Menu>;
  constructor(public menuService: MenuService) { }

  ngOnInit() {
    this.menuItems = this.menuService.getVerticalMenuItems();
    this.menuItems = this.menuItems.filter(item => item.parentId == this.menuParentId);
  }

  onClick(menuId: number) {
    this.menuService.toggleMenuItem(menuId);
    this.menuService.closeOtherSubMenus(this.menuService.getVerticalMenuItems(), menuId);
  }

}
