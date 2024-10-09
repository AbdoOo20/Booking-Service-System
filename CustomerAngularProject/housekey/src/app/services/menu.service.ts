import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Location } from '@angular/common';  
import { Menu } from '@models/menu.model';
import { DomHandlerService } from './dom-handler.service';
import { horizontalMenuItems, verticalMenuItems } from '../common/data/menu.data';

@Injectable()
export class MenuService {

  constructor(private location: Location, private router: Router, private domHandlerService: DomHandlerService) { }

  public getVerticalMenuItems(): Array<Menu> {
    return verticalMenuItems;
  }

  public getHorizontalMenuItems(): Array<Menu> {
    return horizontalMenuItems;
  }

  public expandActiveSubMenu(menu: Array<Menu>) {
    let url = this.location.path();
    let routerLink = decodeURIComponent(url);
    let activeMenuItem = menu.filter(item => item.routerLink === routerLink);
    if (activeMenuItem[0]) {
      let menuItem = activeMenuItem[0];
      while (menuItem.parentId != 0) {
        let parentMenuItem = menu.filter(item => item.id == menuItem.parentId)[0];
        menuItem = parentMenuItem;
        this.toggleMenuItem(menuItem.id);
      }
    }
  }

  public toggleMenuItem(menuId: number) {
    let menuItem = this.domHandlerService.winDocument.getElementById('menu-item-' + menuId);
    let subMenu = this.domHandlerService.winDocument.getElementById('sub-menu-' + menuId);
    if (subMenu) {
      if (subMenu.classList.contains('show')) {
        subMenu.classList.remove('show');
        menuItem!.classList.remove('expanded');
      }
      else {
        subMenu.classList.add('show');
        menuItem!.classList.add('expanded');
      }
    }
  }

  public closeOtherSubMenus(menu: Array<Menu>, menuId: number) {
    let currentMenuItem = menu.filter(item => item.id == menuId)[0];
    menu.forEach(item => {
      if ((item.id != menuId && item.parentId == currentMenuItem.parentId) || (currentMenuItem.parentId == 0 && item.id != menuId)) {
        let subMenu = this.domHandlerService.winDocument.getElementById('sub-menu-' + item.id);
        let menuItem = this.domHandlerService.winDocument.getElementById('menu-item-' + item.id);
        if (subMenu) {
          if (subMenu.classList.contains('show')) {
            subMenu.classList.remove('show');
            menuItem!.classList.remove('expanded');
          }
        }
      }
    });
  }

  public closeAllSubMenus() {
    verticalMenuItems.forEach(item => {
      let subMenu = this.domHandlerService.winDocument.getElementById('sub-menu-' + item.id);
      let menuItem = this.domHandlerService.winDocument.getElementById('menu-item-' + item.id);
      if (subMenu) {
        if (subMenu.classList.contains('show')) {
          subMenu.classList.remove('show');
          menuItem!.classList.remove('expanded');
        }
      }
    });
  }


}