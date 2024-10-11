import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SharedService {
  // BehaviorSubject initialized with null or empty value
  private bookingDataSubject = new BehaviorSubject<any>(null);
  
  constructor() {}

  // Method to set data
  setData(data: any): void {
    this.bookingDataSubject.next(data);  // Emit the new data
  }

  // Method to get data as an observable
  getData(): Observable<any> {
    return this.bookingDataSubject.asObservable();
  }
}
