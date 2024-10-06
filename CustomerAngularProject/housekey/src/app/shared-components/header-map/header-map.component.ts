import { Component, Input, OnInit, SimpleChange, ViewChild } from '@angular/core';
import { GoogleMap, GoogleMapsModule, MapInfoWindow, MapMarker } from '@angular/google-maps';
import { RouterModule } from '@angular/router';
import { Property } from '@models/app.models';
import { AppService } from '@services/app.service';
import { Settings, SettingsService } from '@services/settings.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CurrencyPipe, NgClass } from '@angular/common';

@Component({
    selector: 'app-header-map',
    standalone: true,
    imports: [
        RouterModule,
        GoogleMapsModule,
        MatProgressSpinnerModule,
        NgClass,
        CurrencyPipe
    ],
    templateUrl: './header-map.component.html',
    styleUrls: ['./header-map.component.scss']
})
export class HeaderMapComponent implements OnInit {
    @Input('locations') locations: Array<any> = [];
    @Input('contentOffsetToTop') contentOffsetToTop: boolean = false;
    @Input('fullscreen') fullscreen: boolean = false;
    @ViewChild(GoogleMap) map: GoogleMap;
    @ViewChild(MapInfoWindow) infoWindow: MapInfoWindow;
    center: google.maps.LatLngLiteral = { lat: 40.678178, lng: -93.944158 };
    zoom: number = 5;
    markerOptions: google.maps.MarkerOptions = {
        draggable: false,
        animation: google.maps.Animation.BOUNCE,
    };
    mapStyles: any = [
        {
            "featureType": "all",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "saturation": 36
                },
                {
                    "color": "#000000"
                },
                {
                    "lightness": 40
                }
            ]
        },
        {
            "featureType": "all",
            "elementType": "labels.text.stroke",
            "stylers": [
                {
                    "visibility": "on"
                },
                {
                    "color": "#000000"
                },
                {
                    "lightness": 16
                }
            ]
        },
        {
            "featureType": "all",
            "elementType": "labels.icon",
            "stylers": [
                {
                    "visibility": "off"
                }
            ]
        },
        {
            "featureType": "administrative",
            "elementType": "geometry.fill",
            "stylers": [
                {
                    "color": "#000000"
                },
                {
                    "lightness": 20
                }
            ]
        },
        {
            "featureType": "administrative",
            "elementType": "geometry.stroke",
            "stylers": [
                {
                    "color": "#000000"
                },
                {
                    "lightness": 17
                },
                {
                    "weight": 1.2
                }
            ]
        },
        {
            "featureType": "administrative",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#8b9198"
                }
            ]
        },
        {
            "featureType": "landscape",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#000000"
                },
                {
                    "lightness": 20
                }
            ]
        },
        {
            "featureType": "landscape",
            "elementType": "geometry.fill",
            "stylers": [
                {
                    "color": "#323336"
                }
            ]
        },
        {
            "featureType": "landscape.man_made",
            "elementType": "geometry.stroke",
            "stylers": [
                {
                    "color": "#414954"
                }
            ]
        },
        {
            "featureType": "poi",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#000000"
                },
                {
                    "lightness": 21
                }
            ]
        },
        {
            "featureType": "poi",
            "elementType": "geometry.fill",
            "stylers": [
                {
                    "color": "#2e2f31"
                }
            ]
        },
        {
            "featureType": "road",
            "elementType": "labels.text.fill",
            "stylers": [
                {
                    "color": "#7a7c80"
                }
            ]
        },
        {
            "featureType": "road.highway",
            "elementType": "geometry.fill",
            "stylers": [
                {
                    "color": "#242427"
                },
                {
                    "lightness": 17
                }
            ]
        },
        {
            "featureType": "road.highway",
            "elementType": "geometry.stroke",
            "stylers": [
                {
                    "color": "#202022"
                },
                {
                    "lightness": 29
                },
                {
                    "weight": 0.2
                }
            ]
        },
        {
            "featureType": "road.arterial",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#000000"
                },
                {
                    "lightness": 18
                }
            ]
        },
        {
            "featureType": "road.arterial",
            "elementType": "geometry.fill",
            "stylers": [
                {
                    "color": "#393a3f"
                }
            ]
        },
        {
            "featureType": "road.arterial",
            "elementType": "geometry.stroke",
            "stylers": [
                {
                    "color": "#202022"
                }
            ]
        },
        {
            "featureType": "road.local",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#000000"
                },
                {
                    "lightness": 16
                }
            ]
        },
        {
            "featureType": "road.local",
            "elementType": "geometry.fill",
            "stylers": [
                {
                    "color": "#393a3f"
                }
            ]
        },
        {
            "featureType": "road.local",
            "elementType": "geometry.stroke",
            "stylers": [
                {
                    "color": "#202022"
                }
            ]
        },
        {
            "featureType": "transit",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#000000"
                },
                {
                    "lightness": 19
                }
            ]
        },
        {
            "featureType": "water",
            "elementType": "geometry",
            "stylers": [
                {
                    "color": "#000000"
                },
                {
                    "lightness": 17
                }
            ]
        },
        {
            "featureType": "water",
            "elementType": "geometry.fill",
            "stylers": [
                {
                    "color": "#202124"
                }
            ]
        }
    ];
    mapOptions: google.maps.MapOptions = {
        styles: this.settingsService.settings.theme == 'orange-dark' ? this.mapStyles : null,
        mapTypeControl: false,
        fullscreenControl: true,
        streetViewControl: true
    }
    property: Property | null;
    public settings: Settings;

    constructor(public settingsService: SettingsService, public appService: AppService) {
        this.settings = this.settingsService.settings;
    }

    ngOnInit(): void {
        if (this.contentOffsetToTop) {
            setTimeout(() => {
                this.settings.contentOffsetToTop = this.contentOffsetToTop;
            });
        }
    }

    ngOnChanges(changes: { [propKey: string]: SimpleChange }) {
        if (changes.locations) {
            if (!changes.locations.isFirstChange()) {
                //reset map position on filter
                this.zoom = 5;
                this.center = { lat: 40.678178, lng: -93.944158 };
                this.markerOptions.animation = google.maps.Animation.BOUNCE;
                setTimeout(() => {
                    this.markerOptions = { ...this.markerOptions, animation: null }
                }, 1000);
            }
        }
    }

    ngOnDestroy() {
        setTimeout(() => {
            this.settings.contentOffsetToTop = false;
        });
    }

    onMapReady() {
        setTimeout(() => {
            this.markerOptions = { ...this.markerOptions, animation: null }
        }, 1000);
    }

    onMapClick(e: any) {
        console.log(e);
    }

    onMarkerClick(e: any, propertyId: number, marker: MapMarker) {
        this.center = { lat: e.latLng.lat(), lng: e.latLng.lng() };
        this.infoWindow.open(marker);
        if (this.zoom < 11) {
            this.zoom = 11;
        }
        this.property = null;
        setTimeout(() => {
            this.appService.getPropertyById(propertyId).subscribe(res => {
                this.property = res;
            });
        }, 500);
    }

    onZoomChanged() {
        this.zoom = this.map.getZoom()!;
    }

}
