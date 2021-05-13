import React, { Component } from "react";
import { connect } from "react-redux";
import UserStoreEntry from "./userStoreEntry";

class UserStoreTable extends Component {
  state = {};

  render() {
    return (
      <div>
        <article className="card mb-3">
          <header
            className="card-header pl-3"
            style={{ backgroundColor: "rgba(69, 136, 236, 0.125)" }}
          >
            <h5>{this.props.title}</h5>
          </header>
          <div className="table-responsive">
            <table className="table table-hover">
              <tbody>
                <tr>
                  <th scope="col">Store name:</th>
                  <th scope="col">Description:</th>
                  <th> </th>
                </tr>
                {this.props.stores?.map((store) => (
                  <UserStoreEntry
                    key={store.guid}
                    storeName={store.storeName}
                    description={store.description}
                    guid={store.guid}
                  />
                ))}
              </tbody>
            </table>
          </div>
        </article>

        {/* <article className="card mt-4">
          <header
            className="card-header pl-3"
            style={{ backgroundColor: "rgba(69, 136, 236, 0.125)" }}
          >
            <h5>Owner stores</h5>
          </header>
          <div className="table-responsive">
            <table className="table table-hover">
              <tbody>
                <tr>
                  <th scope="col">Store name:</th>
                  <th scope="col">Description:</th>
                  <th> </th>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> owner </td>
                  <td>
                    <a href="/home" className="btn btn-outline-primary">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> manager </td>
                  <td>
                    <a href="/home" className="btn btn-outline-primary">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </article>

        <article className="card mt-4">
          <header
            className="card-header pl-3"
            style={{ backgroundColor: "rgba(69, 136, 236, 0.125)" }}
          >
            <h5>Manager stores</h5>
          </header>
          <div className="table-responsive">
            <table className="table table-hover">
              <tbody>
                <tr>
                  <th scope="col">Store name:</th>
                  <th scope="col">Description:</th>
                  <th> </th>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> owner </td>
                  <td>
                    <a href="/home" className="btn btn-outline-primary">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
                <tr>
                  <td>
                    <p className="title mb-0">store name goes here </p>
                  </td>
                  <td> manager </td>
                  <td>
                    <a href="/home" className="btn btn-outline-primary">
                      {" "}
                      Details{" "}
                    </a>{" "}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </article> */}
      </div>
    );
  }
}

export default connect()(UserStoreTable);
