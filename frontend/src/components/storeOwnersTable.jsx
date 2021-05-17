import React, { Component } from "react";
import { connect } from "react-redux";
import StoreOwnerEntry from "./storeOwnerEntry";

class StoreOwnerTable extends Component {
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
                  <th scope="col">Owner username:</th>
                  <th> </th>
                </tr>
                {this.props.owners?.map((ownership) => {
                  if (ownership.guid === this.props.myRole.guid) {
                    return null;
                  }
                  if (
                    this.props.subordinates.some(
                      (subordinate) => subordinate.guid === ownership.guid
                    )
                  ) {
                    return (
                      <StoreOwnerEntry
                        isSubordinate={true}
                        key={ownership.guid}
                        username={ownership.userMetaData?.userName}
                        guid={ownership.guid}
                      />
                    );
                  }
                  return (
                    <StoreOwnerEntry
                      isSubordinate={false}
                      key={ownership.guid}
                      username={ownership.userMetaData?.userName}
                      guid={ownership.guid}
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

export default connect()(StoreOwnerTable);
