import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { timeout } from 'rxjs/operators';

@Component({
  selector: 'app-confirmation-email',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirmation-email.component.html',
})
export class ConfirmationEmailComponent {
  private _httpClient = inject(HttpClient);
  private _apiURL = "http://localhost:18105/api/Account";
  public message: string | null = null;

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit(): void {
    // Subscribe to query parameters to get userId and token
    this.route.queryParams.subscribe(params => {
      const userId = params['userId'];
      let token = params['token'];
      
      if (userId && token) {
        token = token.replace(/ /g, '+');
        this.confirmEmail(userId, token);
      } else {
        this.message = 'Invalid confirmation link.';
      }
    });
  }

  getConfirmEmail(userId: string, token: string): Observable<any> {
    return this._httpClient.get(`${this._apiURL}/ConfirmEmail?userId=${userId}&token=${token}`, { observe: 'response' });
  }

  confirmEmail(userId: string, token: string): void {
    console.log("this is confirm email");
    this.getConfirmEmail(userId, token).subscribe({
      next: (response) => {
        if (response.status === 200) {
          this.message = 'Email confirmed successfully! You can now log in.';
          setTimeout(() => this.router.navigate(['/login']), 3000);
        } else {
          this.message = 'Unexpected response status: ' + response.status;
        }
      },
      error: (error) => {
        console.error('API error:', error);
        this.message = 'Email confirmation failed. Please check your link or try again.';
      }
    });
  }
}
