import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router, RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatSlideToggleModule,
    MatTooltipModule,
    FlexLayoutModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  hide = true;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router,
    private snackBar: MatSnackBar,
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(6)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false] 
    });
  }

  onLoginFormSubmit() {
    if (this.loginForm.valid) {
      const loginData = {
        username: this.loginForm.value.username,
        password: this.loginForm.value.password,
        rememberMe: this.loginForm.value.rememberMe
      };

      // Call login API
      this.http.post('http://localhost:5285/api/Account/Login', loginData)
        .subscribe({
          next: (response) => {
            console.log('Login successful:', response);
            this.errorMessage = '';
            this.snackBar.open('Login successful!', '×', {
              panelClass: 'success',
              verticalPosition: 'top',
              duration: 3000
            });
            this.router.navigate(['/home']); // Navigate to home page on success
          },
          error: (error: HttpErrorResponse) => {
            console.error('Login failed:', error);
            if (error.status === 400) {
              this.errorMessage = 'Invalid username or password.';
              this.snackBar.open(this.errorMessage, '×', {
                panelClass: 'error',
                verticalPosition: 'top',
                duration: 3000
              });
            } else {
              this.errorMessage = 'An unexpected error occurred. Please try again later.';
              this.snackBar.open(this.errorMessage, '×', {
                panelClass: 'error',
                verticalPosition: 'top',
                duration: 3000
              });
            }
          }
        });
    } else {
      this.snackBar.open('Please fill in all required fields correctly.', '×', {
        panelClass: 'error',
        verticalPosition: 'top',
        duration: 3000
      });
    }
  }
}
