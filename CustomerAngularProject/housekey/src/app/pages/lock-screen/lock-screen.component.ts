import { DatePipe } from '@angular/common';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { Router, RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
import { DomHandlerService } from '@services/dom-handler.service';

@Component({
  selector: 'app-lock-screen',
  standalone: true,
  imports: [
    RouterModule,
    ReactiveFormsModule,
    FlexLayoutModule,
    MatSidenavModule,
    MatIconModule,
    MatButtonModule,
    DatePipe
  ],
  templateUrl: './lock-screen.component.html',
  styleUrl: './lock-screen.component.scss',
  encapsulation: ViewEncapsulation.None
})
export class LockScreenComponent implements OnInit {
  public date: any = new Date();
  public timerInterval: any;
  public form: FormGroup;

  constructor(public fb: FormBuilder, public router: Router, private domHandlerService: DomHandlerService) { }

  ngOnInit() {
    if (this.domHandlerService.isBrowser) {
      this.timerInterval = setInterval(() => {
        this.date = new Date();
      }, 1000);
    }
    this.form = this.fb.group({
      password: [null, Validators.compose([Validators.required, Validators.minLength(3)])]
    });
  }

  ngAfterViewInit() {
    this.domHandlerService.hidePreloader();
  }

  ngOnDestroy() {
    clearInterval(this.timerInterval);
  }

  public onSubmit(values: Object): void {
    if (this.form.valid) {
      this.router.navigate(['/']);
    }
  }

}
