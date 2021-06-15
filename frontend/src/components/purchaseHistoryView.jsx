import React, { Component } from "react";
import PurchaseHistoryEntry from "./PurchaseHistoryEntry";
class PurchaseHistoryView extends Component {
  state = {};
  render() {
    return (
      <div className="col-6">
        <article className="card mb-3">
          <header
            className="card-header pl-3"
            style={{ backgroundColor: "rgba(69, 136, 236, 0.125)" }}
          >
            <h5>Purchase history</h5>
          </header>
          <div
            className="table-responsive "
            style={{
              display: "block",
              maxHeight: "700px",
              overflowY: "scroll",
            }}
          >
            <table className="table table-hover">
              <tbody>
                {this.props.purchases.map((purchase, index) => (
                  <PurchaseHistoryEntry
                    key={index}
                    hrefPrefix={this.props.hrefPrefix}
                    purchase={purchase}
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

export default PurchaseHistoryView;
