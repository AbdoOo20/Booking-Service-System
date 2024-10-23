import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PassTokenWithHeaderService } from './pass-token-with-header.service';
import { observableToBeFn } from 'rxjs/internal/testing/TestScheduler';
@Injectable({
  providedIn: "root",
})
export class ServiceForConfirmation {
  private endPoint = "https://lilynightapi.runasp.net/api";

  constructor(
    private http: HttpClient,
    private PassTokenWithHeaderService: PassTokenWithHeaderService
  ) {}

  getServiceName(id: number): Observable<any> {
    return this.http.get<any>(
      this.endPoint + "/Services/GetServiceNameByIDV2/" + id,
      { headers: this.PassTokenWithHeaderService.getHeaders() }
    );
  }

  setBankAccount(id: string, bankAccount: any): Observable<any> {
    return this.http.put<any>(
      this.endPoint + "/Customer/SetBanckAccount/" + id,
      bankAccount,
      { headers: this.PassTokenWithHeaderService.getHeaders() }
    );
  }
}