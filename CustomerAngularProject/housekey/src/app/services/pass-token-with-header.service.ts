import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root'
})
export class PassTokenWithHeaderService {
  private token: string | null = null;

  constructor() { }
  private getToken(): string | null {
    if (!this.token) {
      this.token = sessionStorage.getItem("token") || localStorage.getItem("token");
    }
    return this.token;
  }
  public getHeaders(): HttpHeaders {
    const token = this.getToken();
    if (!token) throw new Error('Auth token is missing!');

    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }
}
