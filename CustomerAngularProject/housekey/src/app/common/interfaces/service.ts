export interface Service {
  id: number;
  name: string;
  details: string;
  location: string;
  startTime: string;
  endTime: string;
  providerName: string;
  priceForTheCurrentDay: number;
  initialPayment: number;
  category: string;
  image: string;
  quantity: number;
  _AdminContract: Contract;
  _ProviderContract: Contract;
}

export interface Contract {
  id: number;
  name: string;
  details: string;
}
