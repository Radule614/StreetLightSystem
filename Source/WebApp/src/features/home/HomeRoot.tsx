export const HomeRoot = () => {
  return (
    <section className="p-6 pb-32 max-w-[1000px] mx-auto">
      <h1 className="pb-10 pt-5 text-4xl text-center font-bold">
        Street Light System - SLS
      </h1>
      <p className="mb-7">
        Illuminate your city with efficiency and precision! Street Light System
        is a comprehensive platform designed to empower street light maintenance
        and repair crews to keep your city shining bright. Our advanced features
        simplify the process of monitoring, managing, and maintaining street
        lights, ensuring safer and well-lit neighborhoods for everyone.
      </p>
      <ul className="[&>li]:mb-4">
        <li>
          <b>Real-Time Tracking: </b>
          <div>
            Stay on top of every street light's status in real-time. Our
            interactive map displays the exact location of each street light,
            along with its current status – working, being repaired, or broken.
          </div>
        </li>
        <li>
          <b>Efficient Maintenance: </b>
          <div>
            Streamline maintenance tasks with our user-friendly interface.
            Street Light System allows repair crews to efficiently schedule and
            prioritize maintenance activities, ensuring timely repairs and
            reducing downtime.
          </div>
        </li>
        <li>
          <b>Report and Resolve: </b>
          <div>
            Repair crews can instantly report issues directly from the field.
            Our platform automatically assigns repair requests to the
            appropriate teams, and you can track the progress until the issue is
            resolved.
          </div>
        </li>
        <li>
          <b>Data-Driven Insights: </b>
          <div>
            Access valuable data and analytics to optimize street light
            infrastructure. Make informed decisions based on usage patterns,
            energy consumption, and maintenance histories.
          </div>
        </li>
        <li>
          <b>Collaborative Platform: </b>
          <div>
            Foster better communication and collaboration among repair crews,
            supervisors, and city officials. Share updates, assign tasks, and
            work together seamlessly.
          </div>
        </li>
      </ul>
    </section>
  );
};
