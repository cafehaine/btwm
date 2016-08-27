using System.Runtime.Serialization;

namespace JsonStructures
{
    [DataContract]
    public class Workspace
    {
        [DataMember]
        public string name;

        public Workspace(string name)
        { this.name = name; }
    }

    [DataContract]
    public class Workspaces
    {
        [DataMember]
        public Workspace[] workspaces;

        [DataMember]
        public int focused;

        public Workspaces(Workspace[] workspaces, int focused)
        {
            this.workspaces = workspaces;
            this.focused = focused;
        }
    }

    [DataContract]
    public class Block
    {
        [DataMember]
        public string name;
        [DataMember]
        public string markup;
        [DataMember]
        public string full_text;
        [DataMember]
        public int separator_block_width;
        [DataMember]
        public string short_text; // we won't care about this for now
        [DataMember]
        public string color;
        [DataMember]
        public string background;
        [DataMember]
        public string border;
        [DataMember]
        public bool separator;

        public Block(string name, string markup, string full_text,
            int separator_block_width)
        {
            this.name = name;
            this.markup = markup;
            this.full_text = full_text;
            this.separator_block_width = separator_block_width;
        }
    }

    [DataContract]
    public class BarInfo
    {
        [DataMember]
        public int version;
        [DataMember]
        public bool click_events;
        [DataMember]
        public int stop_signal;
        [DataMember]
        public int cont_signal;

        public BarInfo(int version, bool click_events)
        {
            this.version = version;
            this.click_events = click_events;
        }
    }
}
