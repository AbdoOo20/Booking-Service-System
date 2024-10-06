import { NgClass } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core'; 
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { TranslateModule } from '@ngx-translate/core';
import { DomHandlerService } from '@services/dom-handler.service';
import { Settings, SettingsService } from '@services/settings.service';

@Component({
  selector: 'app-header-video',
  standalone: true,
  imports: [
    FlexLayoutModule,
    RouterModule,
    MatButtonModule,
    TranslateModule,
    NgClass
  ],
  templateUrl: './header-video.component.html',
  styleUrls: ['./header-video.component.scss']
})
export class HeaderVideoComponent implements OnInit {
  @Input('backgroundVideo') backgroundVideo: string; 
  @Input('contentOffsetToTop') contentOffsetToTop: boolean;
  @Input('contentMinHeight') contentMinHeight: number;
  @Input('title') title: string;
  @Input('desc') desc: string;
  @Input('isHomePage') isHomePage:boolean = false;
  @Input('fullscreen') fullscreen: boolean = false;
  public settings: Settings;

  constructor(public settingsService: SettingsService, private domHandlerService: DomHandlerService) { 
    this.settings = this.settingsService.settings;
    setTimeout(() => {
      this.settings.headerBgVideo = true;
    }); 
  }

  ngOnInit(): void {
    if(this.contentOffsetToTop){
      setTimeout(() => {
        this.settings.contentOffsetToTop = this.contentOffsetToTop;
      });
    } 
    var vid = <HTMLVideoElement> this.domHandlerService.winDocument.getElementById("bgVideo");
    vid.muted = true;
  }

  ngOnDestroy(){  
    setTimeout(() => {
      this.settings.headerBgVideo = false; 
      this.settings.contentOffsetToTop = false;
    });  
  }

}
