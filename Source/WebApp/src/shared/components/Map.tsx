import {
  MapContainer,
  Marker,
  Popup,
  TileLayer,
  useMapEvents,
} from "react-leaflet";
import iconRetina from "leaflet/dist/images/marker-icon-2x.png";
import iconShadow from "leaflet/dist/images/marker-shadow.png";
import { icon } from "leaflet";
import { twMerge } from "tailwind-merge";

import defaultPin from "../../assets/default_pin.png";
import brokenPin from "../../assets/broken_pin.png";
import repairPin from "../../assets/repair_pin.png";
import { Button } from "./Button";
import { statusToColor, statusToString } from "../model/pole";
import { useState } from "react";

export interface Location {
  id: string;
  latitude: number;
  longitude: number;
  status: number;
}

const createPinIcon = (pinIcon: any) => {
  return icon({
    iconRetinaUrl: iconRetina,
    iconUrl: pinIcon,
    shadowUrl: iconShadow,

    //small icons
    // iconSize: [25, 36],
    // iconAnchor: [14, 37],

    //medium icons
    iconSize: [30, 44],
    iconAnchor: [15, 45],
    shadowSize: [40, 57],
    shadowAnchor: [11, 58],

    //large icons
    // iconSize: [33, 47],
    // iconAnchor: [17, 46],
    // shadowSize: [44, 60],
    // shadowAnchor: [14, 58]
  });
};

export const MarkerLayer = ({
  locations,
  onActionHandler,
}: {
  locations?: Location[];
  onActionHandler: (id: string) => void;
}) => {
  const defaultMarker = createPinIcon(defaultPin);
  const brokenMarker = createPinIcon(brokenPin);
  const repairMarker = createPinIcon(repairPin);
  const resolveStatusText = (status: number, color: string) => {
    return (
      <>
        <span>Pole is </span>
        <span className={`${color} lowercase`}>{statusToString(status)}.</span>
      </>
    );
  };
  const resolveMarker = (status: number) => {
    switch (status) {
      case 1:
        return brokenMarker;
      case 2:
        return repairMarker;
    }
    return defaultMarker;
  };
  return (
    <>
      {locations?.map((location) => (
        <Marker
          key={location.id}
          position={[location.latitude, location.longitude]}
          icon={resolveMarker(location.status)}
        >
          <Popup>
            <div className="px-2">
              {resolveStatusText(
                location.status,
                statusToColor(location.status)
              )}
            </div>
            <div className="flex justify-center mt-4">
              <Button
                type="button"
                onClick={() => {
                  onActionHandler(location.id);
                }}
                className="py-2 px-3 text-xs"
              >
                Details
              </Button>
            </div>
          </Popup>
        </Marker>
      ))}
    </>
  );
};

export const OverviewLayer = ({
  overviewLocations,
  locations,
}: {
  overviewLocations: Location[];
  locations?: Location[];
}) => {
  const defaultMarker = createPinIcon(defaultPin);
  return (
    <>
      {overviewLocations.map((location) => (
        <Marker
          key={location.id}
          position={[location.latitude, location.longitude]}
          icon={defaultMarker}
        >
          <Popup>
            {locations && (
              <div className="p-1 text-base">
                <div className={statusToColor(0)}>
                  Working: {locations.filter((l) => l.status === 0).length}
                </div>
                <div className={statusToColor(1)}>
                  Broken: {locations.filter((l) => l.status === 1).length}
                </div>
                <div className={statusToColor(2)}>
                  Being repaired:{" "}
                  {locations.filter((l) => l.status === 2).length}
                </div>
              </div>
            )}
          </Popup>
        </Marker>
      ))}
    </>
  );
};

export const MapLayers = ({
  locations,
  onActionHandler,
}: {
  locations?: Location[];
  onActionHandler: (id: string) => void;
}) => {
  const [zoomLevel, setZoomLevel] = useState(14);
  const mapEvents = useMapEvents({
    zoomend: () => {
      setZoomLevel(mapEvents.getZoom());
    },
  });
  return (
    <>
      {zoomLevel <= 11 && (
        <OverviewLayer
          locations={locations}
          overviewLocations={[
            {
              id: "1",
              latitude: 45.251841,
              longitude: 19.837277,
              status: 0,
            },
          ]}
        />
      )}
      {zoomLevel > 11 && (
        <MarkerLayer locations={locations} onActionHandler={onActionHandler} />
      )}
    </>
  );
};

export const Map = ({
  className,
  locations,
  onActionHandler,
}: {
  className: string;
  onActionHandler: (id: string) => void;
  locations?: Location[];
}) => {
  return (
    <MapContainer
      center={[45.251841, 19.837277]}
      zoom={14}
      scrollWheelZoom={false}
      className={twMerge("h-[600px]", className)}
    >
      <TileLayer
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
      />
      <MapLayers locations={locations} onActionHandler={onActionHandler} />
    </MapContainer>
  );
};
