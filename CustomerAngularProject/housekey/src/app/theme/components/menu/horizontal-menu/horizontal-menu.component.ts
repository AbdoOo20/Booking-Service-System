import { Component, OnInit, Input } from '@angular/core'; 
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { RouterModule } from '@angular/router';
import { Menu } from '@models/menu.model';
import { TranslateModule } from '@ngx-translate/core';
import { MenuService } from '@services/menu.service';

@Component({
  selector: 'app-horizontal-menu',
  standalone: true,
  imports: [
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    TranslateModule
  ],
  templateUrl: './horizontal-menu.component.html',
  styleUrls: ['./horizontal-menu.component.scss'],
  providers: [MenuService]
})
export class HorizontalMenuComponent implements OnInit {
  @Input('menuParentId') menuParentId: number;
  public menuItems: Array<Menu>;
  constructor(public menuService: MenuService) { }

  ngOnInit() {
    this.menuItems = this.menuService.getHorizontalMenuItems();
    this.menuItems = this.menuItems.filter(item => item.parentId == this.menuParentId);
  }

}
