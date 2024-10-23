import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: "root",
})
export class ConfirmEmailService {
  private _httpClient = inject(HttpClient);
  private _apiURL = "https://lilynightapi.runasp.net/api/Account";

  constructor() {}

  getConfirmEmail(userId: string, token: string): Observable<any> {
    return this._httpClient.get(
      `${this._apiURL}/ConfirmEmail?userId=${userId}&token=${token}`,
      { observe: "response" }
    );
  }
}
