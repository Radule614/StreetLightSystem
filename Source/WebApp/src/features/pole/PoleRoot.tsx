import { useEffect, useState } from "react";
import { AppState, appStore } from "../../core/store";
import {
  Button,
  Loader,
  Map,
  Pole,
  SuccessActions,
  WarningActions,
} from "../../shared";
import { PoleDetailsModal } from "./PoleDetailsModal";
import { PoleList } from "./PoleList";
import { StartRepairModal } from "./StartRepairModal";
import { useNotification } from "../../core/notification";

export const PoleRoot = () => {
  const poleData = appStore((state: AppState) => state.pole.poles.data);
  const isLoading = appStore((state: AppState) => state.pole.poles.isLoading);
  const fetchPoleData = appStore((state: AppState) => state.pole.fetchPoles);
  const [poleId, setPoleId] = useState<string | null>(null);
  const [areDetailsVisible, setDetailsVisible] = useState(false);
  const [isRepairVisible, setRepairVisible] = useState(false);
  const [outdated, setOutdated] = useState(false);
  const notification = useNotification();

  const refreshData = () => {
    fetchPoleData();
    setOutdated(false);
  };

  useEffect(() => {
    fetchPoleData();
  }, [fetchPoleData]);

  useEffect(() => {
    switch (notification?.action) {
      case WarningActions.PoleStatusChanged:
        setOutdated(true);
        break;
      case SuccessActions.StartRepairSuccess:
        fetchPoleData();
        break;
      case SuccessActions.EndRepairSuccess:
        fetchPoleData();
        break;
    }
  }, [fetchPoleData, notification]);

  return (
    <section className="p-6 pb-32 relative">
      <h2 className="pb-3 text-xl">Poles</h2>
      {outdated && (
        <div className="flex justify-end sticky top-1 z-30 mb-5 pr-1">
          <Button
            variant="warn"
            className="text-xs py-2 px-3"
            onClick={refreshData}
          >
            Refresh data
          </Button>
        </div>
      )}
      {isLoading && <Loader size="lg" className="flex justify-center pt-10" />}
      {!isLoading && (
        <>
          <Map
            className="border-light-3 border-[1px] h-[80vh]"
            onActionHandler={(id: string) => {
              setPoleId(id);
              setDetailsVisible(true);
            }}
            locations={poleData?.map((pole) => ({
              id: pole.id,
              latitude: pole.latitude,
              longitude: pole.longitude,
              status: pole.status,
            }))}
          />
          {poleData && (
            <PoleList
              className="mt-10"
              poles={poleData}
              onDetailsAction={(pole: Pole) => {
                setPoleId(pole.id);
                setDetailsVisible(true);
              }}
              onRepairAction={(pole: Pole) => {
                setPoleId(pole.id);
                setRepairVisible(true);
              }}
            />
          )}
        </>
      )}

      <StartRepairModal
        poleId={poleId ?? ""}
        isVisible={isRepairVisible}
        onClose={() => {
          setRepairVisible(false);
        }}
      />

      <PoleDetailsModal
        poleId={poleId ?? ""}
        isVisible={areDetailsVisible}
        onRepairAction={() => {
          setRepairVisible(true);
        }}
        onClose={() => {
          setDetailsVisible(false);
          setPoleId(null);
        }}
      />
    </section>
  );
};
