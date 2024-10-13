import { HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PassTokenWithHeaderService {

  constructor() { }

  public getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token'); // Ensure token exists
    if (!token) throw new Error('Auth token is missing!');
    
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }
}
