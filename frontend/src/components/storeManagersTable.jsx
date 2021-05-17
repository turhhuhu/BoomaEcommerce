import React, { Component } from "react";
import { connect } from "react-redux";
import StoreManagerEntry from "./storeManagerEntry";

class StoreManagersTable extends Component {
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
            <table className="table table-hover table-striped table-bordered">
              <tbody>
                <tr>
                  <th scope="col">Manager username:</th>
                  <th scope="col">Can add product:</th>
                  <th scope="col">Can remove product:</th>
                  <th scope="col">Can edit product:</th>
                  <th scope="col">Can view other managers/owners:</th>
                  <th> </th>
                  <th> </th>
                </tr>
                {this.props.managers?.map((managementship) => {
                  if (managementship.guid === this.props.myRole.guid) {
                    return null;
                  }
                  if (
                    this.props.subordinates.some(
                      (subordinate) => subordinate.guid === managementship.guid
                    )
                  ) {
                    return (
                      <StoreManagerEntry
                        isSubordinate={true}
                        key={managementship.guid}
                        username={managementship.userMetaData?.userName}
                        permissions={managementship.permissions}
                        guid={managementship.guid}
                      />
                    );
                  }
                  return (
                    <StoreManagerEntry
                      isSubordinate={false}
                      key={managementship.guid}
                      username={managementship.userMetaData?.userName}
                      permissions={managementship.permissions}
                      guid={managementship.guid}
                    />
                  );
                })}
              </tbody>
            </table>
          </div>
        </article>
      </div>
    );
  }
}

export default connect()(StoreManagersTable);
