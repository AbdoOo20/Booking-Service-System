import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Router, RouterModule } from '@angular/router';
import { FlexLayoutModule } from '@ngbracket/ngx-layout';
// Import the auth services for Google and Facebook
// import { AuthService } from 'angularx-social-login';
// import { GoogleLoginProvider, FacebookLoginProvider } from 'angularx-social-login';

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

// export class LoginComponent implements OnInit {
//   public loginForm: FormGroup;
//   public hide = true;
//   constructor(public fb: FormBuilder, public router: Router) { }

//   ngOnInit() {
//     this.loginForm = this.fb.group({
//       username: [null, Validators.compose([Validators.required, Validators.minLength(6)])],
//       password: [null, Validators.compose([Validators.required, Validators.minLength(6)])],
//       rememberMe: false
//     });
//   }

//   public onLoginFormSubmit(): void {
//     if (this.loginForm.valid) {
//       this.router.navigate(['/']);
//     }
//   }

//}

// export class LoginComponent {
//   loginForm: FormGroup;
//   hide = true; // For toggling password visibility

//   constructor(
//     private fb: FormBuilder,
//     private http: HttpClient,
//     private router: Router
//   ) {
//     // Initialize the form
//     this.loginForm = this.fb.group({
//       username: ['', [Validators.required]],  // Using username instead of email
//       password: ['', [Validators.required, Validators.minLength(6)]],
//       rememberMe: [false]
//     });
//   }

//   onLoginFormSubmit() {
//     if (this.loginForm.valid) {
//       const loginData = {
//         username: this.loginForm.value.username,
//         password: this.loginForm.value.password,
//         rememberMe: this.loginForm.value.rememberMe
//       };

//       // Call the API to login
//       this.http.post('http://localhost:5285/api/Account/Login', loginData)
//         .subscribe({
//           next: (response) => {
//             // Handle success response
//             console.log('Login successful:', response);
//             this.router.navigate(['/dashboard']);  // Redirect to dashboard after login
//           },
//           error: (error) => {
//             // Handle error response
//             console.error('Login failed:', error);
//           }
//         });
//     } else {
//       console.log('Form is invalid');
//     }
//   }
// }


// export class LoginComponent {
//   loginForm: FormGroup;
//   public hide = true;
//   constructor(
//     private fb: FormBuilder,
//     private http: HttpClient,
//     private router: Router,
//     //private authService: AuthService  // For Google and Facebook login
//   ) {
//     // Initialize the form
//     this.loginForm = this.fb.group({
//       username: ['', [Validators.required]],  // Using username instead of email
//     });
//   }

//   // Form submission
//   onLoginFormSubmit() {
//     if (this.loginForm.valid) {
//       const loginData = {
//         username: this.loginForm.value.username
//       };

//       // Call your API to handle form-based login
//       this.http.post('http://localhost:5285/api/Account/Login', loginData)
//         .subscribe({
//           next: (response) => {
//             console.log('Login successful:', response);
//             this.router.navigate(['/dashboard']);  // Redirect after login
//           },
//           error: (error) => {
//             console.error('Login failed:', error);
//           }
//         });
//     }
//   }

  // // Login with Google
  // loginWithGoogle() {
  //   this.authService.signIn(GoogleLoginProvider.PROVIDER_ID).then(user => {
  //     const token = user.idToken;  // Use the Google token
  //     this.http.post('http://localhost:5285/api/Account/ExternalLogin', { token })
  //       .subscribe({
  //         next: (response) => {
  //           console.log('Google login successful:', response);
  //           this.router.navigate(['/dashboard']);  // Redirect after login
  //         },
  //         error: (error) => {
  //           console.error('Google login failed:', error);
  //         }
  //       });
  //   });
  // }

  // // Login with Facebook
  // loginWithFacebook() {
  //   this.authService.signIn(FacebookLoginProvider.PROVIDER_ID).then(user => {
  //     const token = user.authToken;  // Use the Facebook token
  //     this.http.post('http://localhost:5285/api/Account/ExternalLogin', { token })
  //       .subscribe({
  //         next: (response) => {
  //           console.log('Facebook login successful:', response);
  //           this.router.navigate(['/dashboard']);  // Redirect after login
  //         },
  //         error: (error) => {
  //           console.error('Facebook login failed:', error);
  //         }
  //       });
  //   });
  // }
//}

export class LoginComponent {
  loginForm: FormGroup;
  hide = true;
  errorMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required]], // Username field
      password: ['', [Validators.required, Validators.minLength(6)]], // Password field
    });
  }

  onLoginFormSubmit() {
    if (this.loginForm.valid) {
      const loginData = {
        username: this.loginForm.value.username,
        password: this.loginForm.value.password
      };

      // Call your API to handle login
      this.http.post('http://localhost:5285/api/Account/Login', loginData)
        .subscribe({
          next: (response) => {
            console.log('Login successful:', response);
            this.errorMessage = '';
            this.router.navigate(['/home']); // Redirect to home page on success
          },
          error: (error) => {
            console.error('Login failed:', error);
            this.errorMessage = 'Invalid username or password'; // Display error message
          }
        });
    }
  }

  loginWithGoogle() {
    // Logic for logging in with Google
    window.location.href = 'https://your-backend-url.com/api/auth/google'; // Replace with actual API endpoint
  }

  loginWithFacebook() {
    // Logic for logging in with Facebook
    window.location.href = 'https://your-backend-url.com/api/auth/facebook'; // Replace with actual API endpoint
  }
}