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
          <div
            className="table-responsive "
            style={{
              display: "block",
              maxHeight: "300px",
              overflowY: "scroll",
            }}
          >
            <table className="table table-hover table-striped">
              <tbody>
                <tr>
                  <th scope="col">Store name:</th>
                  <th scope="col">Description:</th>
                  <th> </th>
                </tr>
                {this.props.stores?.map((store) => (
                  <UserStoreEntry
                    key={store.storeGuid}
                    storeName={store.storeName}
                    description={store.description}
                    guid={store.storeGuid}
                  />
                ))}
              </tbody>
            </table>
          </div>
        </article>
      </div>
    );
  }
}

export default connect()(UserStoreTable);
