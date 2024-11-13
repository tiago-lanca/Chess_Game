using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chess_Game.src.Project.PiecesType
{
    public class PieceConverter : JsonConverter<Piece>
    {
        public override Piece? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                if (root.TryGetProperty("Type", out JsonElement typeElement))
                {
                    int typeValue = typeElement.GetInt16();
                    PieceType type = (PieceType)typeValue;

                    return type switch
                    {
                        PieceType.Pawn => JsonSerializer.Deserialize<Pawn>(root.GetRawText(), options),
                        PieceType.Bishop => JsonSerializer.Deserialize<Bishop>(root.GetRawText(), options),
                        PieceType.Knight => JsonSerializer.Deserialize<Knight>(root.GetRawText(), options),
                        PieceType.Rook => JsonSerializer.Deserialize<Rook>(root.GetRawText(), options),
                        PieceType.Queen => JsonSerializer.Deserialize<Queen>(root.GetRawText(), options),
                        PieceType.King => JsonSerializer.Deserialize<King>(root.GetRawText(), options),

                        _ => throw new JsonException("Tipo de Peça Desconhecida")
                    };
                }
                else throw new JsonException("Tipo de Peça Desconhecida");

            }
        }

        public override void Write(Utf8JsonWriter writer, Piece value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case Pawn pawn:
                    JsonSerializer.Serialize(writer, pawn, options);
                    break;

                case Rook rook:
                    JsonSerializer.Serialize(writer, rook, options);
                    break;

                case Bishop bishop:
                    JsonSerializer.Serialize(writer, bishop, options);
                    break;

                case Knight knight:
                    JsonSerializer.Serialize(writer, knight, options);
                    break;

                case Queen queen:
                    JsonSerializer.Serialize(writer, queen, options);
                    break;

                case King king:
                    JsonSerializer.Serialize(writer, king, options);
                    break;

                default:
                    throw new JsonException("Tipo de peça desconhecida.");
            }
            //JsonSerializer.Serialize(writer, (object)value, options);

        }
    }
}
